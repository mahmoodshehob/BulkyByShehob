using BulkySh.DataAccess.Data;
using BulkySh.DataAccess.Repository;
using BulkySh.DataAccess.Repository.IRepository;
using BulkySh.Models.Models;
using BulkySh.Models.ViewModels;
using BulkySh.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using static BulkySh.Utility.SD;

namespace BulkyShWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	//[Authorize(Roles = SD.Roles.Admin)]

	public class UserController : Controller
	{


		private readonly ApplicationDbContext _db;
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<ApplicationUser> _roleManager;




		public UserController(ApplicationDbContext db, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<ApplicationUser> roleManager)
		{
			_db = db;
			_unitOfWork = unitOfWork;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public IActionResult Index()
		{
			return View();
		}


		public IActionResult RoleManagment(string? userID)
		{	
			var roleId = _db.UserRoles.FirstOrDefault(ur=>ur.UserId == userID).RoleId;

			RoleManagmentVM roleVM = new RoleManagmentVM()
			{
				ApplicationUser = _db.ApplicationUsers.Include(u=>u.Company).FirstOrDefault(u => u.Id == userID),
				RoleList = _db.Roles.Select(i => new SelectListItem {
				Value = i.Id,
				Text = i.Name
				}),
				CompanyList = _db.Companies.Select(i => new SelectListItem
				{
					Value = i.Id.ToString(),
					Text = i.Name
				})
			};

			roleVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(ur => ur.Id == roleId).Name;

			return View(roleVM);
		}

		[HttpPost]
		public IActionResult RoleManagment(RoleManagmentVM roleManagmentVM)
		{

			string oldRole = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == roleManagmentVM.ApplicationUser.Id))
					.GetAwaiter().GetResult().FirstOrDefault();

			ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == roleManagmentVM.ApplicationUser.Id);


			if (!(roleManagmentVM.ApplicationUser.Role == oldRole))
			{
				//a role was updated
				if (roleManagmentVM.ApplicationUser.Role == SD.Roles.Company)
				{
					applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
				}
				if (oldRole == SD.Roles.Company)
				{
					applicationUser.CompanyId = null;
				}
				_unitOfWork.ApplicationUser.Update(applicationUser);
				_unitOfWork.Save();

				_userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
				_userManager.AddToRoleAsync(applicationUser, roleManagmentVM.ApplicationUser.Role).GetAwaiter().GetResult();

			}
			else
			{
				if (oldRole == SD.Roles.Company && applicationUser.CompanyId != roleManagmentVM.ApplicationUser.CompanyId)
				{
					applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
					_unitOfWork.ApplicationUser.Update(applicationUser);
					_unitOfWork.Save();
				}
			}

			return RedirectToAction("Index");
		}

		///////////////////////////////////////////////////////////////////


		//[HttpPost]
		//public IActionResult RoleManagment(RoleManagmentVM roleManagmentVM)
		//{
		//	Test(roleManagmentVM);
		//	try
		//	{
		//		string RoleID = _db.UserRoles.FirstOrDefault(u => u.UserId == roleManagmentVM.ApplicationUser.Id).RoleId;

		//		string oldRole = _db.Roles.FirstOrDefault(u => u.Id == RoleID).Id;

		//		if (!(roleManagmentVM.ApplicationUser.Role == oldRole))
		//		{
		//			ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == roleManagmentVM.ApplicationUser.Id);

		//			//a role was updated
		//			if (roleManagmentVM.ApplicationUser.Role == SD.Roles.Company)
		//			{
		//				applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
		//			}
		//			if (oldRole == SD.Roles.Company)
		//			{
		//				applicationUser.CompanyId = null;
		//			}
		//			_unitOfWork.ApplicationUser.Update(applicationUser);
		//			_unitOfWork.Save();

		//			_userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
		//			_userManager.AddToRoleAsync(applicationUser, roleManagmentVM.ApplicationUser.Role).GetAwaiter().GetResult();

		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		string message = ex.Message;
		//		if (ex.InnerException != null)
		//		{
		//			string details = ex.InnerException.ToString();
		//		}
		//	}




		//	return RedirectToAction("Index");
		//}


		//private void Test(RoleManagmentVM roleManagmentVM) 
		//{
		//	string RoleID = _db.UserRoles.FirstOrDefault(u => u.UserId == roleManagmentVM.ApplicationUser.Id).RoleId;

		//	string oldRole = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u=>u.Id== roleManagmentVM.ApplicationUser.Id)).GetAwaiter().GetResult().FirstOrDefault();




		//}


		#region API CALLS

		[HttpGet]
		public IActionResult GetAll()
		{
			List<ApplicationUser> objUserList = _db.ApplicationUsers.Include(u => u.Company).ToList();
			
			var userRoles = _db.UserRoles.ToList();
			
			var roles = _db.Roles.ToList();

			foreach (var user in objUserList) 
			{
				var roleID = userRoles.FirstOrDefault(r => r.UserId == user.Id).RoleId;
				
				user.Role = roles.FirstOrDefault(rn => rn.Id == roleID).Name;

				if (user.Company == null)
				{
					user.Company = new Company() { Name = "" };
				}
			}

			return Json(new { data = objUserList });
		}

		[HttpPost]
		public IActionResult LockUnlock([FromBody] string ID)
		{
			var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == ID);

			if (objFromDb == null)
			{
				return Json(new { success = false, Message = "Error while Locking/Unlocking" });
			}

			string status;
			if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
			{
				objFromDb.LockoutEnd = DateTime.Now;
				status = "Locked Successful";
			}
			else
			{
				objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
				status = "Unlocked Successful";
			}

			_db.SaveChanges();
			//return Json(new { success = true, Message = $"{status} Successful" });
			return Json(new { success = true, Message = status });
		}

		#endregion
	}
}