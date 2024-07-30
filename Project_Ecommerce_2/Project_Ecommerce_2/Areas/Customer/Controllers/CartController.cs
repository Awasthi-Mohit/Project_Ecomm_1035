using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Project_Ecommerce_2.DataAccess.Repository.IRepository;
using Project_Ecommerce_2.Models;
using Project_Ecommerce_2.Models.ViewModels;
using Project_Ecommerce_2.Utility;
using Stripe;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using System.Net.Mail;
using System.Net;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Project_Ecommerce_2.Areas.Customer.Controllers
{
    [Area("Customer")]
    
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private static bool isEmailConfirm = false;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        public CartController(IUnitOfWork unitOfWork,IEmailSender emailSender,UserManager<IdentityUser>userManager) 
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _userManager = userManager;
        }
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public IActionResult Index() //detail of user who login
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) //when user not login and also it made empty objects so that it don't give error
            {
                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = new List<ShoppingCart>()
                };
                return View(ShoppingCartVM);
            }
            //***
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll //data pick from shoppingcart
                (sc => sc.ApplicationUserId == claim.Value, includeProperties: "Product"),// user who login & get data from multiple table
                OrderHeader = new OrderHeader() //empty obt of orderheader class
            };
            ShoppingCartVM.OrderHeader.OrderTotal = 0; //bydefault set 0  
            ShoppingCartVM.OrderHeader.ApplicationUser = // user who login we get detail
                _unitOfWork.ApplicationUser.FirstOrDefault(u=>u.Id== claim.Value); //we get value from claim.value
            foreach (var list in ShoppingCartVM.ListCart) // loop works upto total no. of items in listcart& price is not mapped
            { 
                list.Price = SD.GetPriceBasedOnQuantity(list.Count,list.Product.Price,
                    list.Product.Price50,list.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += list.Price*list.Count;
                if (list.Product.Description.Length>100)
                {
                    list.Product.Description = list.Product.Description.Substring(0,99)+"...";
                }
            }
            //Email
            if(!isEmailConfirm)
            {
                ViewBag.EmailMessage = "Email has been sent kindly verify your email!";
                ViewBag.EmailCSS = "text-success";
                isEmailConfirm = false;
            }
            else
            {
                ViewBag.EmailMessage = "Email must be confirm for authorize customer!";
                ViewBag.EmailCSS = "text-danger";
            }
            return View(ShoppingCartVM);
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value);
            if(user == null)
            {
                ModelState.AddModelError(string.Empty, "Email Empty!!");
            }
            else
            {
                //Email
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                isEmailConfirm = true;
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult plus(int id)
        {
            var cart = _unitOfWork.ShoppingCart.FirstOrDefault(sc=>sc.Id== id);
            cart.Count += 1;
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult minus(int id)
        {
            var cart = _unitOfWork.ShoppingCart.FirstOrDefault(sc=>sc.Id == id);
            cart.Count -= 1;
            if (cart.Count == 1)
                cart.Count = 1;
            else
                cart.Count -= 1;
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult delete(int id)
        {
            var cart = _unitOfWork.ShoppingCart.FirstOrDefault(c=>c.Id== id);
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();
            //session
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim!=null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(sc =>sc.ApplicationUserId==claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Summary()
        {
            var claimsIdentity =(ClaimsIdentity)User.Identity; //to check that user is login or not
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM = new ShoppingCartVM() //listcart & Orderheader
            {//user who login from shoppingcart & also product key for product table data.cart details inthis
                ListCart = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new OrderHeader() //empty objt for initialization
            };//user detail who login and set detail in Applicationuser
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value);
            foreach(var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count,//bcz price is not mapped
                    list.Product.Price,list.Product.Price50,list.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Count + list.Price);
                list.Product.Description = SD.ConvertToRawHtml(list.Product.Description);
            }
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            
            return View(ShoppingCartVM);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value);
            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value, includeProperties: "Product");
            ShoppingCartVM.OrderHeader.Carrier = "";
            ShoppingCartVM.OrderHeader.TransactionId = "";
            ShoppingCartVM.OrderHeader.TrackingNumber = "";
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusPending;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();
            foreach (var list in ShoppingCartVM.ListCart) //user who login have 3 items in cart and loop works three time &O.D=3rec
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = list.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = list.Price,
                    Count = list.Count
                };
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }
            _unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
            _unitOfWork.Save();
            //Session empty
            HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, 0);
            #region Stripe Process
            if (stripeToken == null) //Companyuser
            {
                ShoppingCartVM.OrderHeader.PaymentDueDate = DateTime.Today.AddDays(30);
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
            }
            else
            { //Payment Process
                var options = new ChargeCreateOptions()
                {
                    Amount = Convert.ToInt32(ShoppingCartVM.OrderHeader.OrderTotal),
                    Currency = "usd",
                    Description = "Order Id:" + ShoppingCartVM.OrderHeader.Id,
                    Source = stripeToken
                };
                //Payment
                var service = new ChargeService();
                Charge charge = service.Create(options);
                if (charge.BalanceTransactionId == null)
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
                else
                    ShoppingCartVM.OrderHeader.TransactionId = charge.BalanceTransactionId;
                if(charge.Status.ToLower()=="succeeded") //payment comes in account/s (bank account is linked through stripe)
                {
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
                    ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
                }
                _unitOfWork.Save();
            }
            #endregion
            return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id });
        }
        public IActionResult OrderConfirmation(int id)
        {
            try
            {

                string smtpServer = "smtp-mail.outlook.com";
                int smtpPort = 587;
                string smtpUsername = "mohitawasthi4575@outlook.com";
                string smtpPassword = "Awasthi221@?";

                // Create the email message
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(smtpUsername);
                mail.To.Add("mohitawasthi4575@gmail.com");
                mail.Subject = "Order Confirmation";



                mail.Body = "Your Order No.7002 has been successfully orderd." +
                    "You wil recive another SMS when the your product is out to deliver .For more information Contact us ";


                SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
                smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                smtpClient.EnableSsl = true;
                // Send the email
                smtpClient.Send(mail);

                ViewBag.Message = "Email sent successfully!";
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred: " + ex.Message;
            }
            //SMS//
            var accountSid = "AC01025ccdf07dba70753854a541f6054d";
            var authToken = "866b50663e2c9fbd0931a7b462e27eda";
            TwilioClient.Init(accountSid, authToken);

            var messageOptions = new CreateMessageOptions(
              new PhoneNumber("+917807138198"));
            messageOptions.From = new PhoneNumber("+12192667931");
            messageOptions.Body = "Your order has been successfully Processed.";


            var message = MessageResource.Create(messageOptions);
            Console.WriteLine(message.Body);
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            return View(id);
        }

    }
}
