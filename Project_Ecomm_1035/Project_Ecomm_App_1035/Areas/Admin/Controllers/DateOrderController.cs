using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Ecomm_App_1035.DataAccess.Repository;
using Project_Ecomm_App_1035.DataAccess.Repository.IRepository;
using Project_Ecomm_App_1035.Model.ViewModels;
using Project_Ecomm_App_1035.Utility;
using System.Globalization;

namespace Project_Ecomm_App_1035.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee + "," + SD.Role_Individual)]

    public class DateOrderController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        public DateOrderController(IUnitOfWork unitOfWork)
        {
            _unitofwork = unitOfWork;
        }
        public IActionResult Index()
        {
            var viewModel = new DateOrder
            {
                OrderHeaders = _unitofwork.OrderHeader.GetAll(),
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                SelectedInterval = "empty"
            };
            return View(viewModel);

        }


        [HttpPost]
        [ActionName("Index")]
        public IActionResult Index(DateTime startDate, DateTime endDate,DateOrder date)
        {
            DateOrder viewModel = new DateOrder();
            viewModel.StartDate = startDate;
            viewModel.EndDate = endDate;
            viewModel.orders = _unitofwork.OrderHeader.GetAll();
            viewModel.orders = viewModel.orders
            .Where(o => o.OrderDate >= startDate && o.OrderDate <=
            endDate)
            .ToList();
            if (date.SelectedInterval == "Weekly")
            {
                viewModel.WeeklySummary = viewModel.orders
                .GroupBy(o =>
                CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(o.OrderDate,
                CalendarWeekRule.FirstDay, DayOfWeek.Sunday))
                .ToDictionary(g => "Week " + g.Key, g => g.Count());
            }
            if (date.SelectedInterval == "Monthly")
            {
                viewModel.MonthlySummary = viewModel.orders
                .GroupBy(o => o.OrderDate.Month)
                .ToDictionary(g =>
                CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key), g =>
                g.Count());
            }
            return View("Index", viewModel);
        }
    }
}
