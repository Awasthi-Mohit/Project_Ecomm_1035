using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Ecomm_App_1035.DataAccess.Repository.IRepository;
using Project_Ecomm_App_1035.Model;
using Project_Ecomm_App_1035.Model.ViewModels;
using Project_Ecomm_App_1035.Utility;
using System.Globalization;

namespace Project_Ecomm_App_1035.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee +","+ SD.Role_Individual)]
    public class AllorderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public AllorderController(IUnitOfWork unitOfWork)
        {
                _unitOfWork=unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _unitOfWork.OrderHeader.GetAll() });
        }
       
        public IActionResult ViewDetail(int id)
        {
            var orderHeader=_unitOfWork.OrderHeader.FirstOrDefault(e=>e.id==id);    
            return View(orderHeader);   
        }
        //[HttpPost]
        //[ActionName("Index")]
        //public IActionResult Index(DateTime startDate, DateTime endDate, DateOrder date)
        //{
        //    DateOrder viewModel = new DateOrder();
        //    viewModel.StartDate = startDate;
        //    viewModel.EndDate = endDate;
        //    viewModel.SelectedInterval = date.SelectedInterval;
        //    viewModel.SelectedStatus = date.SelectedStatus;    

        //    viewModel.OrderHeaders = _unitOfWork.OrderHeader.GetAll();
        //    viewModel.OrderHeaders = viewModel.OrderHeaders.Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate).ToList();

        //    if (date.SelectedInterval == "Monthly")
        //    {
        //        // Group orders by month and get the count of orders in each month.
        //        viewModel.MonthlySummary = viewModel.OrderHeaders
        //            .GroupBy(o => o.OrderDate.Month)
        //            .ToDictionary(g => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key), g => g.Count());
        //    }
        //    else if (date.SelectedInterval == "Weekly")
        //    {
               
        //    }

            // Filter orders based on selected order status.
            //if (date.SelectedStatus == "Pending")
            //{
            //    viewModel.OrderHeaders = viewModel.OrderHeaders.Where(o => o.Status == "Pending").ToList();
            //}
            //else if (date.SelectedStatus == "Approved")
            //{
            //    viewModel.OrderHeaders = viewModel.OrderHeaders.Where(o => o.Status == "Approved").ToList();
            //}

        //    return View("Index", viewModel);
        //}

    }
}