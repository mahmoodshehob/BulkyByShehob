using System.Security.Claims;
using BulkySh.DataAccess.Repository.IRepository;
using BulkySh.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BulkySh.Utility;
using Microsoft.AspNetCore.Http;



namespace BulkyShWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

		private void updateCartCount()
		{
			var cliamdIdentity = (ClaimsIdentity)User.Identity;
			var cliam = cliamdIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (cliam != null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cliam.Value).Count());
            }
            else 
            {
				HttpContext.Session.Clear();
			}
		}

		public IActionResult Index()
        {
            updateCartCount();

			IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");

			return View(productList);
        }

        public IActionResult Details(int productId)
        {
			

			var shoppingCart = _unitOfWork.ShoppingCart.Get(u => u.ProductId == productId);

            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart()
                {
                    Count = 0

                };
            }


            ShoppingCart cart = new ShoppingCart()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category"),
                Count = shoppingCart.Count,
                ProductId = productId
            };

            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {

            var cliamdIdentity = (ClaimsIdentity)User.Identity;
            var userId = cliamdIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(
                u => u.ApplicationUserId == userId &&
                u.ProductId == shoppingCart.ProductId);

            if (cartFromDb != null)
            {
                // shopping cart exists

                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
				_unitOfWork.Save();
			}
            else
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
				_unitOfWork.Save();
                updateCartCount();

			}


			TempData["success"] = "Cart updated Successfully";
            

			return RedirectToAction(nameof(Index));
		}

		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}