﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project_Ecomm_App_1035.DataAccess.Repository.IRepository;
using Project_Ecomm_App_1035.Model.ViewModels;
using Project_Ecomm_App_1035.Utility;

namespace Project_Ecomm_App_1035.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles =SD.Role_Admin + "," +SD.Role_Employee)]

	public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment; 
        }
        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            return Json(new {data=productList});
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var productInDb = _unitOfWork.Product.Get(id);
            if (productInDb == null)
                return Json(new {success=false,message="Something went wrong!!!"});
            // for image delete
            var webRootPath = _webHostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRootPath,productInDb.ImageUrl.Trim('\\'));
            if(System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            _unitOfWork.Product.Remove(productInDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "data deleted successfully!!!" });
        }

        #endregion
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
               Product=new Model.Product(),
               CategoryList = _unitOfWork.Category.GetAll().Select(cl=>new SelectListItem()
               {
                   Text= cl.Name,
                   Value=cl.Id.ToString()
               }),
               CoverTypeList=_unitOfWork.CoverType.GetAll().Select(cl=>new SelectListItem()
               {
                   Text= cl.Name,
                   Value=cl.Id.ToString(),
               })
            };
            if (id == null) return View(productVM); // for create
            //for edit
            productVM.Product=_unitOfWork.Product.Get(id.GetValueOrDefault());
            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if(ModelState.IsValid) 
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var files= HttpContext.Request.Form.Files;
                if(files.Count()>0)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(files[0].FileName);
                    var uploads = Path.Combine(webRootPath, @"images\products");
                    if(productVM.Product.Id!=0)
                    {
                        var imagesExists = _unitOfWork.Product.Get(productVM.Product.Id).ImageUrl;
                        productVM.Product.ImageUrl=imagesExists;
                    }
                    if (productVM.Product.ImageUrl!=null) 
                    {
                        var imagePath = Path.Combine(webRootPath, productVM.Product.ImageUrl.Trim('\\'));
                        if(System.IO.File.Exists(imagePath)) 
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }
                else
                {
                    if(productVM.Product.Id!=0) 
                    {
                        var imageExists  = _unitOfWork.Product.Get(productVM.Product.Id).ImageUrl;
                        productVM.Product.ImageUrl = imageExists ;
                    }
                }
                if (productVM.Product.Id == 0)
                    _unitOfWork.Product.Add(productVM.Product);
                else
                    _unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                 productVM = new ProductVM()
                {
                    Product = new Model.Product(),
                    CategoryList = _unitOfWork.Category.GetAll().Select(cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.Id.ToString(),
                    }),
                    CoverTypeList = _unitOfWork.CoverType.GetAll().Select(cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.Id.ToString(),
                    })
                };
                if(productVM.Product.Id!= 0) 
                {
                    productVM.Product = _unitOfWork.Product.Get(productVM.Product.Id);
                }
                return View(productVM);
            }
        }
    }
}
