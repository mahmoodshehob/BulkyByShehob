using BulkySh.DataAccess.Data;
using BulkySh.DataAccess.Repository.IRepository;
using BulkySh.Models.Models;
using BulkySh.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyShWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Roles.Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return View(objCategoryList);
        }



        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Create(Category NewCatg)
        {
            if (NewCatg.Name == NewCatg.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The Display Order can't Exactly match the name");
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
                _unitOfWork.Category.Add(NewCatg);
                _unitOfWork.Save();
                TempData["Success"] = "Category Created Successfully";
                return RedirectToAction("Index", "Category");

            }
            return View();

        }



        public IActionResult Edit(int ID)
        {
            if (ID == 0 || ID == null)
            {
                return NotFound();

            }
            Category categoryFromDB = _unitOfWork.Category.Get(u => u.Id == ID);
            if (categoryFromDB == null)
            {
                return NotFound();

            }
            return View(categoryFromDB);
        }

        [HttpPost]
        public IActionResult Edit(Category updatedCatg)
        {
            if (updatedCatg.Name == updatedCatg.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The Display Order can't Exactly match the name");
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
                _unitOfWork.Category.Update(updatedCatg);
                _unitOfWork.Save();
                TempData["Success"] = "Category Updated Successfully";
                return RedirectToAction("Index", "Category");

            }
            return View();

        }


        public IActionResult Delete(int? ID)
        {
            if (ID == 0 || ID == null)
            {
                return NotFound();

            }
            Category categoryFromDB = _unitOfWork.Category.Get(u => u.Id == ID);
            if (categoryFromDB == null)
            {
                return NotFound();

            }
            return View(categoryFromDB);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? ID)
        {

            Category categoryFromDB = _unitOfWork.Category.Get(c => c.Id == ID);
            if (categoryFromDB == null)
            {
                return NotFound();

            }
            _unitOfWork.Category.Remove(categoryFromDB);
            _unitOfWork.Save();
            TempData["Success"] = "Category Deleted Successfully";
            return RedirectToAction("Index", "Category");

        }
    }
}