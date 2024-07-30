using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Ecommerce_2.DataAccess.Data;
using Project_Ecommerce_2.DataAccess.Repository.IRepository;
using Project_Ecommerce_2.Models;
using Project_Ecommerce_2.Utility;

namespace Project_Ecommerce_2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin+","+SD.Role_Employee)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        public UserController(IUnitOfWork unitofWork,ApplicationDbContext context)
        {
            _unitOfWork = unitofWork;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region API's
        [HttpGet]
        public IActionResult GetAll() 
        { 
            var userList = _context.ApplicationUsers.ToList();//AspNetUsers
            var roles = _context.Roles.ToList(); //AspNetRoles
            var userRoles = _context.UserRoles.ToList(); //AspNetUserRoles
            foreach (var user in userList) 
            {
                var roleId  = userRoles.FirstOrDefault(r =>r.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(r => r.Id == roleId).Name;

                if(user.Company==null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }
            //Remove Admin Role User
            var adminUser = userList.FirstOrDefault(u => u.Role == SD.Role_Admin);
            userList.Remove(adminUser);
            return Json(new {data = userList});
        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            bool isLocked = false; //default value of islocked is false
            var userInDb = _context.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (userInDb == null)
                return Json(new { success = false, message = "Something went wrong while lock or unlock users" });
            if (userInDb!=null && userInDb.LockoutEnd>DateTime.Now)
            {
                userInDb.LockoutEnd = DateTime.Now;
                isLocked = false;
            }
            else
            {
                userInDb.LockoutEnd = DateTime.Now.AddYears(100);
                isLocked = true;
            }
            _context.SaveChanges();
            return Json(new { success = true, message = isLocked == true ? 
                "User successfully locked" : "User successfully Unlocked" });
        }

        #endregion
    }
}
