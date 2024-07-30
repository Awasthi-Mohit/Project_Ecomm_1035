using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Ecommerce_2.DataAccess.Repository.IRepository;
using Project_Ecommerce_2.Models;

namespace Project_Ecommerce_2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
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
        #region API's
        [HttpGet]
        public IActionResult GetAll() 
        {
            var categoryList = _unitOfWork.Category.GetAll();
            return Json(new {data = categoryList});
        }
        public IActionResult Delete (int id)
        {
            var categoryinDb = _unitOfWork.Category.Get(id);
            if (categoryinDb == null)
                return Json(new { success = false, message = "Something went wrong while delete data!!" });
            _unitOfWork.Category.Remove(categoryinDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "data deleted successfully!!" });


        }
        #endregion
        public IActionResult Upsert(int? id) 
        {
            Category category = new Category();
            if(id == null) return View(category);
            category = _unitOfWork.Category.Get(id.GetValueOrDefault());
            if (category == null) return NotFound();
            return View(category);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Upsert(Category category) 
        {
            if (category == null) return NotFound();
            if (!ModelState.IsValid) return View(category);
            if(category.Id==0) 
                _unitOfWork.Category.Add(category);
            else
                _unitOfWork.Category.Update(category);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));

        }
    }
}
