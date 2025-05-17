using BulkySh.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkySh.Models.ViewModels
{
    public class RoleManagmentVM
    {
		public ApplicationUser ApplicationUser { get; set; }
		public IEnumerable<SelectListItem> RoleList { get; set; }
		public IEnumerable<SelectListItem> CompanyList { get; set; }
		public string CurrentRole { get; set; }
	}
}
