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

    public class CompanyController : Controller
    {


        private readonly IUnitOfWork _unitOfWork;



        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return View(objCompanyList);
        }



        [HttpGet]
        public IActionResult Upsert(int? ID)
        {



            if (ID == 0 || ID == null)
            {
                // Create

                return View(new Company());
            }
            else
            {
                // Update

                Company company = _unitOfWork.Company.Get(u => u.Id == ID);

                return View(company);
            }
        }


        [HttpPost]
        public IActionResult Upsert(Company upsertComp)
        {

            if (ModelState.IsValid)
            {

                if (upsertComp.Id == 0 || upsertComp.Id == null)
                {
                    _unitOfWork.Company.Add(upsertComp);
                    _unitOfWork.Company.Save();
                    TempData["Success"] = "Company Created Successfully";
                    return RedirectToAction("Index", "Company");
                }
                else
                {
                    _unitOfWork.Company.Update(upsertComp);
                    _unitOfWork.Company.Save();
                    TempData["Success"] = "Company Updated Successfully";
                    return RedirectToAction("Index", "Company");

                }
            }

            else
            {
                return View(upsertComp);
            }
        }

        public IActionResult Delete(int? ID)
        {
            if (ID == 0 || ID == null)
            {
                return NotFound();

            }
            Company CompanyFromDB = _unitOfWork.Company.Get(u => u.Id == ID);
            if (CompanyFromDB == null)
            {
                return NotFound();

            }
            return View(CompanyFromDB);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? ID)
        {

            Company CompanyFromDB = _unitOfWork.Company.Get(c => c.Id == ID);
            if (CompanyFromDB == null)
            {
                return NotFound();

            }
            _unitOfWork.Company.Remove(CompanyFromDB);
            _unitOfWork.Company.Save();
            TempData["Success"] = "Company Deleted Successfully";
            return RedirectToAction("Index", "Company");

        }





        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();


            return Json(new { data = objCompanyList });
        }

        [HttpDelete]
        public IActionResult DeleteApi(int? id)
        {
            var CompanyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);

            if (CompanyToBeDeleted == null)
            {
                return Json(new { success = false, Message = "Error while deleting" });
            }

            _unitOfWork.Company.Remove(CompanyToBeDeleted);
            _unitOfWork.Company.Save();

            return Json(new { success = true, Message = "Delete Successfuly" });

        }

        #endregion



    }
}