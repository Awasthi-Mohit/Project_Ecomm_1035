using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Ecommerce_2.DataAccess.Repository.IRepository;
using Project_Ecommerce_2.Models;
using Project_Ecommerce_2.Utility;

namespace Project_Ecommerce_2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
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
            return Json(new {data=_unitOfWork.Company.GetAll()});
        }
        public IActionResult Delete(int id)
        {
            var companyInDb = _unitOfWork.Company.Get(id);
            if (companyInDb == null)
                return Json(new {success=false,message ="Something went wrong while deleting data"});
            _unitOfWork.Company.Remove(companyInDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "data deleted successfully" });
        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            Company company = new Company();
            if(id == null) return View(company);
            company = _unitOfWork.Company.Get(id.GetValueOrDefault());
            if (company == null) return NotFound();
            return View(company);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (company == null) return BadRequest();
            if (!ModelState.IsValid) return View(company);
            if (company.Id == 0)
                _unitOfWork.Company.Add(company);
            else
                _unitOfWork.Company.Update(company);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

    }
}
