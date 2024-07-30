using Microsoft.AspNetCore.Mvc;
using Project_Ecomm_App_1035.DataAccess.Repository;
using Project_Ecomm_App_1035.DataAccess.Repository.IRepository;
using Project_Ecomm_App_1035.Utility;

namespace Project_Ecomm_App_1035.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class RefundController : Controller
    {

        private readonly IUnitOfWork _unitofwork;
        public RefundController(IUnitOfWork unitOfWork)
        {
                _unitofwork=unitOfWork; 
        }
        public IActionResult Index()
        {
            return View();
        }

        #region Api
        public IActionResult GetAll()
        {
            var ApprovedOrder = _unitofwork.OrderHeader.GetAll();
            if (ApprovedOrder != null)
            {
                ApprovedOrder = ApprovedOrder.Where(e => e.OrderStatus == SD.orderstatusrefund).ToList();
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
