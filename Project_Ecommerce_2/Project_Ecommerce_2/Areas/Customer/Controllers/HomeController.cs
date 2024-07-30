using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Ecommerce_2.DataAccess.Data;
using Project_Ecommerce_2.DataAccess.Repository;
using Project_Ecommerce_2.DataAccess.Repository.IRepository;
using Project_Ecommerce_2.Models;
using Project_Ecommerce_2.Models.ViewModels;
using Project_Ecommerce_2.Utility;
using System.Diagnostics;
using System.Security.Claims;

namespace Project_Ecommerce_2.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context,ILogger<HomeController>logger, IUnitOfWork unitOfWork)
        {
            _context = context;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(string? search) //when user is login
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity; //get id from user.login
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier); 
            if (claim != null) // it means when user login then v check total no. of items
                    {
                var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
            }
            var allProducts = from p in _context.Products select p;
            if (search !=null)
            {
                allProducts = allProducts.Where(m=>m.Title!.Contains(search));
                var check = allProducts.ToList();
                if (check.Count() == 0)
                {
                    var authorProd = from p in _context.Products select p;
                    authorProd = authorProd.Where(m=>m.Author==search);
                    return View(authorProd);
                }
            }
            return View(allProducts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Details(int id) 
        {
            var productInDb = _unitOfWork.Product.FirstOrDefault
                (x => x.Id ==id, includeProperties: "Category,CoverType");
            if (productInDb == null) return NotFound();
            var shoppingCart = new ShoppingCart()
            {
                Product = productInDb,
                ProductId = productInDb.Id
            };
            //Session
            var claimsIdentity = (ClaimsIdentity)User.Identity; //get id from user.login
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null) // it means when user login then v check total no. of items
            {
                var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
            }

            return View(shoppingCart);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize]
        public IActionResult Details (ShoppingCart shoppingCart) 
        {
            shoppingCart.Id = 0;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim == null)return NotFound();
            shoppingCart.ApplicationUserId = claim.Value;

            var shoppingCartFromDb = _unitOfWork.ShoppingCart.FirstOrDefault
                (sc => sc.ApplicationUserId == claim.Value &&
                sc.ProductId == shoppingCart.ProductId);
            if (shoppingCartFromDb == null)
                //Add
                _unitOfWork.ShoppingCart.Add(shoppingCart);
            else
                //Update
                shoppingCartFromDb.Count += shoppingCart.Count;
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
      
    }
}