using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_Ecomm_App_1035.DataAccess.Repository.IRepository;
using Project_Ecomm_App_1035.Model;
using Project_Ecomm_App_1035.Model.ViewModels;
using Project_Ecomm_App_1035.Utility;
using System.Diagnostics;
using System.Security.Claims;

namespace Project_Ecomm_App_1035.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(string search)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var  claim=claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim != null)
            {
                var count=_unitOfWork.Shopping.GetAll(m=>m.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount,count);
            }

            var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");

            if (!string.IsNullOrEmpty(search))
            {
                productList = productList.Where(m => m.Title.Contains(search) || m.Author.Contains(search));
            }
            return View(productList);
        }

        public IActionResult Details(int id)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _unitOfWork.Shopping.GetAll(m => m.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
            }
            var productInDb=_unitOfWork.Product.FirstOrDefault(x => x.Id == id,includeProperties: "Category,CoverType");
            if (productInDb == null) return NotFound();
            var shoppingCart = new ShopingCart()
            {
                Product =productInDb,
                ProductId=productInDb.Id
            };
            return View(shoppingCart);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        //[Authorize]
        public IActionResult Details(ShopingCart shopingCart)
        {
            shopingCart.Id = 0;
            if (ModelState.IsValid)
            {
                var cliamIdentity = (ClaimsIdentity)User.Identity;
                var claim = cliamIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null) return NotFound();
                shopingCart.ApplicationUserId = claim.Value;

                var shopingCartFromDb=_unitOfWork.Shopping.FirstOrDefault
                    (sc=>sc.ApplicationUserId== claim.Value && sc.ProductId==shopingCart.ProductId);
                if(shopingCartFromDb == null)
                    _unitOfWork.Shopping.Add(shopingCart);
                else
                shopingCartFromDb.Count += shopingCart.Count;
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var productInDb = _unitOfWork.Product.FirstOrDefault(x => x.Id ==shopingCart.ProductId, includeProperties: "Category,CoverType");
                if (productInDb == null) return NotFound();
                var shoppingCart = new ShopingCart()
                {
                    Product = productInDb,
                    ProductId = productInDb.Id
                };
                return View(shoppingCart);

            }

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
    }
}