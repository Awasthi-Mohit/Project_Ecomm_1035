using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project_Ecommerce_2.DataAccess.Data;
using Project_Ecommerce_2.DataAccess.Repository.IRepository;
using Project_Ecommerce_2.Models;
using Project_Ecommerce_2.Models.ViewModels;
using Project_Ecommerce_2.Utility;

namespace Project_Ecommerce_2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitofWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitofWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitofWork = unitofWork;
            _webHostEnvironment = webHostEnvironment;

        }
        public IActionResult Index()
        {
            return View();
        }
        #region API's
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitofWork.Product.GetAll(includeProperties: "Category,CoverType");
            return Json(new { data = productList });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var productInDb = _unitofWork.Product.Get(id);
            if(productInDb == null)
                return Json(new {success = false,message="Something went wrong while delete data!"});
            //image delete
            var webRootPath = _webHostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRootPath, productInDb.ImageUrl.Trim('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            _unitofWork.Product.Remove(productInDb);
            _unitofWork.Save();
            return Json(new { success = true, message = "data successfully deleted" });
        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitofWork.Category.GetAll().
                Select(cl => new SelectListItem()
                {
                    Text = cl.Name,
                    Value = cl.Id.ToString(),
                }),
                CoverTypeList = _unitofWork.CoverType.GetAll().
                Select(cl => new SelectListItem()
                {
                    Text = cl.Name,
                    Value = cl.Id.ToString()
                })
            };
            if (id == null) return View(productVM); //create
            productVM.Product = _unitofWork.Product.Get(id.GetValueOrDefault());//edit
            return View(productVM);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]

        public IActionResult Upsert(ProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count() > 0)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(files[0].FileName);
                    var uploads = Path.Combine(webRootPath, @"images\products");
                    if (productVM.Product.Id != 0) //edit
                    {
                        var imageExists = _unitofWork.Product.Get(productVM.Product.Id).ImageUrl;//find path from db
                        productVM.Product.ImageUrl = imageExists;//path set
                    }
                    if (productVM.Product.ImageUrl != null)//edit & delete previous one
                    {
                        var imagePath = Path.Combine(webRootPath, productVM.Product.ImageUrl.Trim('\\'));
                        if (System.IO.File.Exists(imagePath))//if file is made then we want to update it
                        {
                            System.IO.File.Delete(imagePath);//which delete existing file(image) and add new one
                        }

                    }//now save code for image(create)
                    using (var filestream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);//the path we set now got saved 
                    }//store the below path of image in the db
                    productVM.Product.ImageUrl = @"\images\products\" + fileName + extension;

                }

                else//if we want to make changes in others(price) can't update image
                {
                    if (productVM.Product.Id != 0)//edit
                    {  //find existing image and set the path
                        var imageExists = _unitofWork.Product.Get(productVM.Product.Id).ImageUrl;
                        productVM.Product.ImageUrl = imageExists;
                    }
                }
                if (productVM.Product.Id == 0) //save
                    _unitofWork.Product.Add(productVM.Product);
                else
                    _unitofWork.Product.Update(productVM.Product);
                _unitofWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                productVM = new ProductVM()
                {
                    Product = new Product(),
                    CategoryList = _unitofWork.Category.GetAll().
                    Select(cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.Id.ToString(),
                    }),
                    CoverTypeList = _unitofWork.CoverType.GetAll().
                    Select(cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.Id.ToString()
                    })
                };
                //create
                if (productVM.Product.Id == null) return View(productVM); //create
                productVM.Product = _unitofWork.Product.Get(productVM.Product.Id);//edit
                return View(productVM);

            }

        }
    }
}
