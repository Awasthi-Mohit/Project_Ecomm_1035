using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Project_Ecomm_App_1035.DataAccess.Repository.IRepository;
using Project_Ecomm_App_1035.Model;
using Project_Ecomm_App_1035.Utility;

namespace Project_Ecomm_App_1035.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]


	public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var CategoryList = _unitOfWork.Category.GetAll();
            return Json(new { data = CategoryList });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var categoryInDb = _unitOfWork.Category.Get(id);
            if (categoryInDb == null)
                return Json(new { success = false, message = "Something went wrong while delete data !!" });
            _unitOfWork.Category.Remove(categoryInDb);
            _unitOfWork.Save();
            return Json(new {success = true, message="Data successfully deleted !!"});
        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            Category category = new Category();
            if (id == null) return View(category); //Create
            //for edit
            category = _unitOfWork.Category.Get(id.GetValueOrDefault());
            if (category == null) return NotFound();
            return View(category);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (category == null) return NotFound();
            if(!ModelState.IsValid) return View(category);
            if(category.Id==0)
                _unitOfWork.Category.Add(category);
            else
                _unitOfWork.Category.Update(category);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}

