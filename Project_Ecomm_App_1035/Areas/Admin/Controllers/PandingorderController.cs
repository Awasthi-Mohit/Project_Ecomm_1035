using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Ecomm_App_1035.DataAccess.Repository.IRepository;
using Project_Ecomm_App_1035.Model.ViewModels;
using Project_Ecomm_App_1035.Utility;

namespace Project_Ecomm_App_1035.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee + "," + SD.Role_Individual)]
    public class PandingorderController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        public PandingorderController(IUnitOfWork unitOfWork)
        {
                    _unitofwork=unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetAll(int SelectedProductId) 
        {

            var pending = _unitofwork.OrderHeader.GetAll();
            if(pending != null)
            {
                pending = pending.Where(p => p.OrderStatus == SD.PaymentStatusPending).ToList();
            }
            return Json(new {data=pending});
        }


        public IActionResult Detail(int Id)
        {
            var orderHeader= _unitofwork.OrderHeader.FirstOrDefault(e=>e.id==Id);    
            return View(orderHeader);   
        }
    }
}
