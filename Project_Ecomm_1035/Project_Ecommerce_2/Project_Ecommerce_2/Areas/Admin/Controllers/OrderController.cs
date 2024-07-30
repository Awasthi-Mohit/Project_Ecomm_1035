using Microsoft.AspNetCore.Mvc;
using Project_Ecommerce_2.DataAccess.Repository.IRepository;

namespace Project_Ecommerce_2.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region API's
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _unitOfWork.OrderDetail.GetAll()});
        }
        #endregion


    }
}
