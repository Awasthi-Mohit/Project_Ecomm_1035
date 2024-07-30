using Microsoft.AspNetCore.Mvc;
using Project_Ecommerce_2.DataAccess.Repository;
using Project_Ecommerce_2.DataAccess.Repository.IRepository;

namespace Project_Ecommerce_2.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class AllorderController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        public AllorderController(IUnitOfWork unitOfWork)
        {
               _unitofwork=unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
		[HttpGet]
		public IActionResult GetAll()
		{
			return Json(new { data = _unitofwork.OrderHeader.GetAll() });
		}

		public IActionResult ViewDetail(int id)
		{
			var orderHeader = _unitofwork.OrderHeader.FirstOrDefault(e => e.Id == id);
			return View(orderHeader);
		}
	}
}
