using BulkySh.DataAccess.Data;
using BulkySh.Models.Models;
using BulkySh.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkySh.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ApplicationDbContext _db;

		public DbInitializer(
			UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager,
			ApplicationDbContext db)
		{
			_roleManager = roleManager;
			_userManager = userManager;
			_db = db;
		}

		public void Initialize()
		{


			//migrations if they are not applied
			try
			{
				if (_db.Database.GetPendingMigrations().Count() > 0)
				{
					_db.Database.Migrate();
				}
			}
			catch (Exception ex) { }



			//create roles if they are not created
			if (!_roleManager.RoleExistsAsync(SD.Roles.Customer).GetAwaiter().GetResult())
			{
				_roleManager.CreateAsync(new IdentityRole(SD.Roles.Customer)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SD.Roles.Employee)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SD.Roles.Admin)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SD.Roles.Company)).GetAwaiter().GetResult();


				//if roles are not created, then we will create admin user as well
				_userManager.CreateAsync(new ApplicationUser
				{
					UserName = "Admin",
					Email = "admin@bulky.ly",
					Name = "MAhmood Shehob",
					PhoneNumber = "1112223333",
					StreetAddress = "test 123 Ave",
					State = "IL",
					PostalCode = "23422",
					City = "Yefren"
				}, "ASDasd@123").GetAwaiter().GetResult();


				ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@bulky.ly");
				_userManager.AddToRoleAsync(user, SD.Roles.Admin).GetAwaiter().GetResult();
			}

			return;
		}
	}
}