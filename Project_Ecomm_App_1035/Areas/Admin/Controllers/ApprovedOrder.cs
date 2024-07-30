using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Ecomm_App_1035.DataAccess.Repository;
using Project_Ecomm_App_1035.DataAccess.Repository.IRepository;
using Project_Ecomm_App_1035.Utility;

namespace Project_Ecomm_App_1035.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee + "," + SD.Role_Individual)]

    public class ApprovedOrder : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        public ApprovedOrder(IUnitOfWork unitOfWork)
        {
            _unitofwork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var ApprovedOrder = _unitofwork.OrderHeader.GetAll();
            if (ApprovedOrder != null)
            {
                ApprovedOrder = ApprovedOrder.Where(e => e.OrderStatus == SD.orderstatusApproved).ToList();
            }
            return Json(new { data = ApprovedOrder });
        }
        public IActionResult Detail(int Id)
        {
            var orderHeader = _unitofwork.OrderHeader.FirstOrDefault(p => p.id == Id);
            return View(orderHeader);

        }
    }
}
