using BulkySh.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkySh.Web.ViewComponents
{
	public class ShoppingCartViewComponent : ViewComponent
	{
		private readonly IUnitOfWork _unitOfWork;
		public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		
		public async Task<IViewComponentResult> InvokeAsync()
		{
			var cliamdIdentity = (ClaimsIdentity)User.Identity;
			var cliam = cliamdIdentity.FindFirst(ClaimTypes.NameIdentifier);
			if (cliam != null)
			{
				if (HttpContext.Session.GetInt32(SD.SessionCart) != null)
				{
					HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cliam.Value).Count());
				}
				return View(HttpContext.Session.GetInt32(SD.SessionCart));
			}
			else
			{
				HttpContext.Session.Clear();
				return View(0);
			}
		}
	}
}
