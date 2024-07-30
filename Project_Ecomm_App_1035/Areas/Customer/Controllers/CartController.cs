using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Query.Internal;
using NuGet.Frameworks;
using Project_Ecomm_App_1035.DataAccess.Data;
using Project_Ecomm_App_1035.DataAccess.Repository;
using Project_Ecomm_App_1035.DataAccess.Repository.IRepository;
using Project_Ecomm_App_1035.Model;
using Project_Ecomm_App_1035.Model.ViewModels;
using Project_Ecomm_App_1035.Utility;
using Stripe;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using System.Net.Mail;
using System.Net;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using NuGet.Packaging.Signing;

namespace Project_Ecomm_App_1035.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly ApplicationDbContext _context;
        private static bool isEmailConfirm = false;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(IUnitOfWork unitOfWork, ApplicationDbContext context,IEmailSender emailSender,UserManager<IdentityUser>userManager)
        {
            _unitofwork = unitOfWork;
            _context = context;
            _emailSender=emailSender;
            _userManager=userManager;
        }
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public ShopingCart ShopingCart { get; set; }

        public IActionResult Index()
        {
           var claimsIdentity=(ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claims== null)
            {
                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = new List<ShopingCart>()
                };
                return View(ShoppingCartVM);


            }
            ShoppingCartVM = new ShoppingCartVM()
            {
               ListCart = _unitofwork.Shopping.GetAll(sc => sc.ApplicationUserId == claims.Value, includeProperties: "Product"),
                orderHeader = new OrderHeader()
            };
            

            ShoppingCartVM.orderHeader.OrderTotal= 0;
            ShoppingCartVM.orderHeader.ApplicationUser=_unitofwork.ApplicationUser.FirstOrDefault( an=>an.Id==claims.Value);
            foreach (var List in ShoppingCartVM.ListCart)
            {
                List.price = SD.GetPriceBasedOnQuality(List.Count, List.Product.Price50, List.Product.Price, List.Product.Price100);
                ShoppingCartVM.orderHeader.OrderTotal += (List.price * List.Count);
                if (List.Product.Description.Length > 100)
                {
                    List.Product.Description = List.Product.Description.Substring(0, 99) + "...";
                }


               // Email
                if (!isEmailConfirm)
                {
                    ViewBag.EmailMessage = "Email has been sent  verifey the email";
                    ViewBag.EmailCSS = "text-success";
                    isEmailConfirm = false;

                }
                else
                {
                    ViewBag.EmailMessage = "Email must be matched";
                    ViewBag.EmailCSS = "text-danger";
                }
            }
            return View(ShoppingCartVM);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("index")]
        public async Task<IActionResult> IndexPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _unitofwork.ApplicationUser.FirstOrDefault(au => au.Id == claims.Value);
            if(user== null)
            {
                ModelState.AddModelError(string.Empty, "Email is empty");
            }
            else
            {
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
        public IActionResult Plus(int id) 
        {
            var cart=_unitofwork.Shopping.FirstOrDefault(an=>an.Id==id);
            cart.Count += 1;
            _unitofwork.Save();
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Minus(int id)
        {
            var cart = _unitofwork.Shopping.FirstOrDefault(mo => mo.Id == id);
            if (cart.Count == 1)
                cart.Count = 1;
            else
                cart.Count = -1;
            _unitofwork.Save();
            return RedirectToAction(nameof(Index));

            
        }
        public IActionResult Delete(int id)
        {
            var mohit=_unitofwork.Shopping.FirstOrDefault(M=>M.Id==id);
            _unitofwork.Shopping.Remove(mohit);
            _unitofwork.Save();
            //session
            var climsIdentity = (ClaimsIdentity)User.Identity;
            var cliams = climsIdentity.FindFirst(ClaimTypes.NameIdentifier);
           var count=_unitofwork.Shopping.GetAll(sc=>sc.ApplicationUserId==cliams.Value).ToList().Count;
            HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
            //
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Summary(string ids)
        {
            var climsIdentity = (ClaimsIdentity)User.Identity;
            var cliams = climsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //ShoppingCartVM = new ShoppingCartVM()
            //{
            //    orderHeader = new OrderHeader(),
            //    ListCart = _unitofwork.Shopping.GetAll
            //    (sc => sc.ApplicationUserId == cliams.Value, includeProperties: "Product"),

            //};
                if (ids == null)
                {


                    ShoppingCartVM = new ShoppingCartVM()
                    {

                        ListCart = _unitofwork.Shopping.GetAll(Sc => Sc.ApplicationUserId == cliams.Value, includeProperties: "Product"),
                        orderHeader = new OrderHeader()

                    };
                }
                else
                {
                    ShoppingCartVM = new ShoppingCartVM()
                    {

                        ListCart = _unitofwork.Shopping.GetAll(Sc => ids.Contains(Sc.Id.ToString()), includeProperties: "Product"),
                        orderHeader = new OrderHeader()

                    };
                }

                ShoppingCartVM.orderHeader.ApplicationUser = _unitofwork.ApplicationUser
                .FirstOrDefault(au => au.Id == cliams.Value);
            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.price = SD.GetPriceBasedOnQuality
                    (list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
                ShoppingCartVM.orderHeader.OrderTotal += (list.price * list.Count);
                if (list.Product.Description.Length > 100)
                {
                    list.Product.Description = list.Product.Description.Substring(0, 99) + "..";
                }

            }
            ShoppingCartVM.orderHeader.Name=ShoppingCartVM.orderHeader.ApplicationUser.Name;
            ShoppingCartVM.orderHeader.StreetAddress=ShoppingCartVM.orderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.orderHeader.State=ShoppingCartVM.orderHeader.ApplicationUser.State;
            ShoppingCartVM.orderHeader.City = ShoppingCartVM.orderHeader.ApplicationUser.City;
            ShoppingCartVM.orderHeader.PostCode = ShoppingCartVM.orderHeader.ApplicationUser.PostalCode;
            ShoppingCartVM.orderHeader.PhoneNumber = ShoppingCartVM.orderHeader.ApplicationUser.PhoneNumber;
            return View(ShoppingCartVM);

        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(string stripeToken,string ids)
         {
            var climsIdentity = (ClaimsIdentity)User.Identity;
            var cliams = climsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            
           
            ShoppingCartVM.orderHeader.ApplicationUser = _unitofwork.ApplicationUser.FirstOrDefault(u => u.Id == cliams.Value);
            ShoppingCartVM.ListCart = _unitofwork.Shopping.GetAll(sc => sc.ApplicationUserId == cliams.Value, includeProperties: "Product");
            //these shows the payment status
            ShoppingCartVM.orderHeader.OrderStatus = SD.orderstatuspemding;
            ShoppingCartVM.orderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.orderHeader.OrderDate = DateTime.Now; //for finding the current time and date of the order
            ShoppingCartVM.orderHeader.ApplicationUserId = cliams.Value;
            _unitofwork.OrderHeader.Add(ShoppingCartVM.orderHeader);
            _unitofwork.Save();
            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.price = SD.GetPriceBasedOnQuality(list.Count, list.Product.Price, list.Product.Price50,
                    list.Product.Price100);
                OrderDetails orderDetails=new OrderDetails()
                {
                    ProductId = list.ProductId,
                        OrderHeaderId=ShoppingCartVM.orderHeader.id,
                        Price=list.price,
                        Count=list.Count,
                };
                ShoppingCartVM.orderHeader.OrderTotal += (list.price * list.Count);
                _unitofwork.OrderDetails.Add(orderDetails);
                _unitofwork.Save();

            }
            _unitofwork.Shopping.RemoveRange(ShoppingCartVM.ListCart);
            _unitofwork.Save();
            //httpsession will call for clearing the cart
            HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, 0);
            #region Stripe

            if(stripeToken==null)
            {
                ShoppingCartVM.orderHeader.PaymentDueDate = DateTime.Today.AddDays(30);
                ShoppingCartVM.orderHeader.PaymentStatus = SD.PaymentStatusDelayPayment;
                ShoppingCartVM.orderHeader.OrderStatus = SD.orderstatusApproved;
                
            }
            else
            {
                //payment
                var options = new ChargeCreateOptions()
                {
                    Amount = Convert.ToInt32(ShoppingCartVM.orderHeader.OrderTotal),
                    Currency = "usd",
                    Description = "orderId:" + ShoppingCartVM.orderHeader.id,
                    Source = stripeToken,

                };
                var services = new ChargeService();
                Charge charge=services.Create(options);
                if(charge.BalanceTransactionId==null)
                
                    ShoppingCartVM.orderHeader.PaymentStatus = SD.PaymentstatusRejected;
                    
                else
                
                    ShoppingCartVM.orderHeader.TransationId = charge.BalanceTransactionId;
                if(charge.Status.ToLower()=="succeeded")
                {
                    ShoppingCartVM.orderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    ShoppingCartVM.orderHeader.OrderStatus= SD.orderstatusApproved;
                    ShoppingCartVM.orderHeader.OrderDate = DateTime.Now;
                }
                _unitofwork.Save();
            }
            #endregion
            return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.orderHeader.id });
        }

        public IActionResult RefundOrder()                    
        {
            var climsIdentity = (ClaimsIdentity)User.Identity;
            var cliams = climsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitofwork.Shopping.GetAll
                (sc => sc.ApplicationUserId == cliams.Value, includeProperties: "Product"),
                orderHeader = new OrderHeader()
            };
            ShoppingCartVM.orderHeader.ApplicationUser = _unitofwork.ApplicationUser
                .FirstOrDefault(au => au.Id == cliams.Value);
            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.price = SD.GetPriceBasedOnQuality
                    (list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
                ShoppingCartVM.orderHeader.OrderTotal += (list.price * list.Count);
                if (list.Product.Description.Length > 100)
                {
                    list.Product.Description = list.Product.Description.Substring(0, 99) + "..";
                }

            }
            ShoppingCartVM.orderHeader.Name = ShoppingCartVM.orderHeader.ApplicationUser.Name;
            ShoppingCartVM.orderHeader.StreetAddress = ShoppingCartVM.orderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.orderHeader.State = ShoppingCartVM.orderHeader.ApplicationUser.State;
            ShoppingCartVM.orderHeader.City = ShoppingCartVM.orderHeader.ApplicationUser.City;
            ShoppingCartVM.orderHeader.PostCode = ShoppingCartVM.orderHeader.ApplicationUser.PostalCode;
            ShoppingCartVM.orderHeader.PhoneNumber = ShoppingCartVM.orderHeader.ApplicationUser.PhoneNumber;

            return View(ShoppingCartVM); 
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
            //SMS on Mobile
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

        //public async Task<IActionResult> OrderConfirmation(int id)
        //{
        //    var claimsIdentity = (ClaimsIdentity)User.Identity;
        //    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        //    //session
        //    if (claim != null)
        //    {
        //        var count = _unitofwork.Shopping.GetAll(c => c.ApplicationUserId == claim.Value).ToList().Count;
        //        HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
        //    }
        //    //var user = _unitofwork.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value);
        //    //var userId = await _userManager.GetUserIdAsync(user);
        //    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        //    //await _emailSender.SendEmailAsync(user.Email, "Your Order's Status:",
        //    //    $"Your Order Has Been Confirmed!!!");

        //    //Twilio
        //    //string accountSid = "AC9c7384659ba40c79f002b30d5c-535ff5";
        //    //string authToken = "77aefeb5df4b0b1a779b522cbb040345";
        //    //var phoneNumber1 = _unitOfWork.ApplicationUser.FirstOrDefault(x => x.Id == claim.Value);
        //    //string phoneNumber = phoneNumber1.PhoneNumber;
        //    //TwilioClient.Init(accountSid, authToken);

        //    //var message = MessageResource.Create(
        //    //    body: "Your Order is Confirmed And Your Order's id is:" + id,
        //    //    from: new Twilio.Types.PhoneNumber("+13203825494"),
        //    //    to: phoneNumber
        //    //    );

        //    return View(id);
        //}

    }
}
