using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Ecomm_App_1035.DataAccess.Repository;
using Project_Ecomm_App_1035.DataAccess.Repository.IRepository;
using Project_Ecomm_App_1035.Utility;

namespace Project_Ecomm_App_1035.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
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
        #region 
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
        public IActionResult ViewDetail(int id)
        {
            var orderHeader = _unitofwork.OrderHeader.FirstOrDefault(e => e.id == id);
            return View(orderHeader);
        }
    }
}
