using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Project_Ecomm_App_1035.DataAccess.Data;
using Project_Ecomm_App_1035.DataAccess.Repository.IRepository;
using Project_Ecomm_App_1035.Model;
using Project_Ecomm_App_1035.Utility;
using System.Data;

namespace Project_Ecomm_App_1035.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        public UserController(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
			var userList = _context.ApplicationUsers.ToList();
			var roles = _context.Roles.ToList();
			var userRoles = _context.UserRoles.ToList();
			foreach (var user in userList)
			{
				var roleId = userRoles.FirstOrDefault(r => r.UserId == user.Id).RoleId;
				user.Role = roles.FirstOrDefault(r => r.Id == roleId).Name;
				if (user.CompanyId != null)
				{
					user.Company = new Company()
					{
						Name = _unitOfWork.Company.Get(Convert.ToInt32(user.CompanyId)).Name
					};

				}

				if (user.Company == null)
				{
					user.Company = new Company()
					{
						Name = ""
					};
				}
			}
			var adminUser = userList.FirstOrDefault(v => v.Role == SD.Role_Admin);
			userList.Remove(adminUser);

			return Json(new { data = userList });
		}
		[HttpPost]
		
		public IActionResult LockUnLock([FromBody] string id)
		{
			bool isLock = false;
			var userInDb = _context.ApplicationUsers.FirstOrDefault(u => u.Id == id);
			if (userInDb == null)
				return Json(new { success = false, message = "Error while Lock/UnLock User" });
			if (userInDb != null && userInDb.LockoutEnd > DateTime.Now)
			{
				userInDb.LockoutEnd = DateTime.Now;
				isLock = false;
			}
			else
			{
				userInDb.LockoutEnd = DateTime.Now.AddYears(50);
				isLock = true;
			}
			_context.SaveChanges();
			return Json(new { success = true, message = isLock == true ? "User Successfull Locked" : "User Successfull Unlocked" });
		}

	}
	
		
	#endregion

}
