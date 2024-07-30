using Microsoft.AspNetCore.Mvc;
using Project_Ecommerce_2.DataAccess.Repository.IRepository;
using Project_Ecommerce_2.Utility;

namespace Project_Ecommerce_2.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class CancelController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        public CancelController(IUnitOfWork unitOfWork)
        {
                _unitofwork=unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region Apis
        [HttpGet]
        public IActionResult GetAll()
        {
            var ApprovedOrder = _unitofwork.OrderHeader.GetAll();
            if (ApprovedOrder != null)
            {
                ApprovedOrder = ApprovedOrder.Where(e => e.OrderStatus == SD.OrderStatusCancelled).ToList();
            }
            return Json(new { data = ApprovedOrder });
        }
        #endregion
    }
}
