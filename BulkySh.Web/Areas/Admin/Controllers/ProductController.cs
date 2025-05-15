using BulkySh.DataAccess.Data;
using BulkySh.DataAccess.Repository;
using BulkySh.DataAccess.Repository.IRepository;
using BulkySh.Models.Models;
using BulkySh.Models.ViewModels;
using BulkySh.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace BulkyShWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Roles.Admin)]

    public class ProductController : Controller
    {


        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string wwwRootPath;


        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            wwwRootPath = _webHostEnvironment.WebRootPath;
        }

        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return View(objProductList);
        }

        //
        //example of viewbag ex4785
        //

        //public IActionResult Create()
        //{
        //    IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category
        //        .GetAll().Select(u => new SelectListItem
        //        {
        //            Text = u.Name,
        //            Value = u.Id.ToString()
        //        });

        //    ProductVM productVM = new ProductVM()
        //    {
        //        Product = new Product(),
        //        CategoryList = CategoryList

        //    };


        //    ViewBag.CategoryList = CategoryList;
        //    return View();
        //}

        //
        //example of viewdata ex4786
        //

        //public IActionResult Create()
        //{
        //    IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category
        //        .GetAll().Select(u => new SelectListItem
        //        {
        //            Text = u.Name,
        //            Value = u.Id.ToString()
        //        });

        //    ProductVM productVM = new ProductVM()
        //    {
        //        Product = new Product(),
        //        CategoryList = CategoryList

        //    };


        //    ViewData["CategoryList"] = CategoryList;
        //    return View();
        //}

        [HttpGet]
        public IActionResult Upsert(int? ID)
        {
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });

            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = CategoryList

            };

            if (ID == 0 || ID == null)
            {
                // Create

                return View(productVM);
            }
            else
            {
                // Update

                productVM.Product = _unitOfWork.Product.Get(u => u.Id == ID);

                return View(productVM);
            }
        }


        [HttpPost]
        public IActionResult Upsert(ProductVM upsertProd, IFormFile? file)
        {

            //List<string> ExistTitles = _unitOfWork.Product.GetAll().Select(x => x.Title).ToList();

            //if (ExistTitles.Contains(upsertProd.Product.Title))
            //{
            //    ModelState.AddModelError("Title", "The Title Existed");
            //}

     



            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(upsertProd.Product.ImageUrl))
                    {
                        string olsdImagePath = Path.Combine(wwwRootPath, upsertProd.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(olsdImagePath))
                        {
                            System.IO.File.Delete(olsdImagePath);
                        }
                    
                    }
                    
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    upsertProd.Product.ImageUrl = @"\images\product\" + fileName;
                }



                if (upsertProd.Product.Id == 0 || upsertProd.Product.Id == null)
                {
                    _unitOfWork.Product.Add(upsertProd.Product);
                    _unitOfWork.Product.Save();
                    TempData["Success"] = "Product Created Successfully";
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    _unitOfWork.Product.Update(upsertProd.Product);
                    _unitOfWork.Product.Save();
                    TempData["Success"] = "Product Updated Successfully";
                    return RedirectToAction("Index", "Product");

                }
            }

            else
            {
                upsertProd.CategoryList = _unitOfWork.Category
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(upsertProd);
            }


            
        }



        public IActionResult Create()
        {
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });

            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = CategoryList

            };

            return View(productVM);
        }




        [HttpPost]
        public IActionResult Create(ProductVM NewProd)
        {

            List<string> ExistTitles = _unitOfWork.Product.GetAll().Select(x => x.Title).ToList();

            if (ExistTitles.Contains(NewProd.Product.Title))
            {
                ModelState.AddModelError("Title", "The Title Existed");
            }

            //if (NewCatg.DisplayOrder == null)
            //{
            //    ModelState.AddModelError("DisplayOrder", "The Display Order can't Be Nullable");
            //}


            //if (NewCatg.Name != null && NewCatg.Name.ToLower() == "test")
            //{
            //    ModelState.AddModelError("Name", "Test is an Invalid Value");
            //}


            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(NewProd.Product);
                _unitOfWork.Product.Save();
                TempData["Success"] = "Product Created Successfully";
                return RedirectToAction("Index", "Product");

            }
            else 
            {
                NewProd.CategoryList = _unitOfWork.Category
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });


                return View(NewProd);

            }
        }



        public IActionResult Edit(int ID)
        {
            if (ID == 0 || ID == null) 
            {
                return NotFound();
            
            }

            Product productFromDB = _unitOfWork.Product.Get(u => u.Id == ID);
            if (productFromDB == null)
            {
                return NotFound();

            }
            return View(productFromDB);
        }

        [HttpPost]
        public IActionResult Edit(Product updatedProd)
        {

            List<string> ExistTitles = _unitOfWork.Product.GetAll().Select(x => x.Title).ToList();

            if (ExistTitles.Contains(updatedProd.Title))
            {
                ModelState.AddModelError("Title", "The Title Existed");
            }

            //if (NewCatg.DisplayOrder == null)
            //{
            //    ModelState.AddModelError("DisplayOrder", "The Display Order can't Be Nullable");
            //}


            //if (NewCatg.Name != null && NewCatg.Name.ToLower() == "test")
            //{
            //    ModelState.AddModelError("Name", "Test is an Invalid Value");
            //}


            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(updatedProd);
                _unitOfWork.Product.Save();
                TempData["Success"] = "Product Updated Successfully";
                return RedirectToAction("Index", "Product");
            }

            return View();

        }


        public IActionResult Delete(int? ID)
        {
            if (ID == 0 || ID == null)
            {
                return NotFound();

            }
            Product productFromDB = _unitOfWork.Product.Get(u=> u.Id==ID);
            if (productFromDB == null)
            {
                return NotFound();

            }
            return View(productFromDB);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? ID)
        {

            Product productFromDB = _unitOfWork.Product.Get(c => c.Id==ID);
            if (productFromDB == null)
            {
                return NotFound();

            }
            _unitOfWork.Product.Remove(productFromDB);
            _unitOfWork.Product.Save();
            TempData["Success"] = "Product Deleted Successfully";
            return RedirectToAction("Index", "Product");

        }





        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();


            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult DeleteApi(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);

            if (productToBeDeleted == null)
            {
                return Json(new { success = false, Message = "Error while deleting" });
            }

            string olsdImagePath =
                Path.Combine(wwwRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(olsdImagePath))
            {
                System.IO.File.Delete(olsdImagePath);
            }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Product.Save();

            return Json(new { success = true, Message = "Delete Successfuly" });

        }

        #endregion



    }
}