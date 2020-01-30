using OPIDDaily.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OPIDDaily.DAL;

namespace OPIDDaily.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class MergeController : Controller
    {

        // GET: Merge
        public ActionResult Merge()
        {
            TempData["UploadedFile"] = "";
            // ViewData["MergeStatus"] = "Wait for the Merge Complete message after clicking the Merge button";
            return View();
        }

        [HttpPost]
        public ActionResult UploadOpidDailyFile(FileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var postedFile = Request.Files["File"];

                string fname = postedFile.FileName;

                if (!fname.EndsWith("xlsx"))
                {
                    ModelState.AddModelError("OPIDDailyError", "This is not an Excel xlsx file.");
                    return View("Merge", model);
                }

                List<string> docfiles = FileUploader.UploadFile(postedFile);
                TempData["UploadedFile"] = fname;
                TempData["FileType"] = "OPIDDaily";
                ViewData["UploadedOPIDDailyFile"] = string.Format("Uploaded File: {0}", fname);

                return View("Merge", model);
            }

            ModelState.AddModelError("OPIDDailyError", "Please supply a file name.");
            return View("Merge", model);
        }

        [HttpPost]
        public ActionResult UploadVoidedChecksFile(FileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var postedFile = Request.Files["File"];

                string fname = postedFile.FileName;

                if (!fname.EndsWith("xlsx"))
                {
                    ModelState.AddModelError("VoidedChecksError", "This is not an Excel xlsx file.");
                    return View("Merge", model);
                }

                List<string> docfiles = FileUploader.UploadFile(postedFile);
                TempData["UploadedFile"] = fname;
                TempData["FileType"] = "VoidedChecks";
                ViewData["UploadedVCFile"] = string.Format("Uploaded File: {0}", fname);

                return View("Merge", model);
            }

            ModelState.AddModelError("VoidedChecksError", "Please supply a file name.");
            return View("Merge", model);
        }

        [HttpPost]
        public ActionResult UploadClearedChecksFile(FileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var postedFile = Request.Files["File"];

                string fname = postedFile.FileName;

                if (!fname.EndsWith("xlsx"))
                {
                    ModelState.AddModelError("ClearedChecksError", "This is not an Excel xlsx file.");
                    return View("Merge", model);
                }

                List<string> docfiles = FileUploader.UploadFile(postedFile);
                TempData["UploadedFile"] = fname;
                TempData["FileType"] = "ClearedChecks";
                ViewData["UploadedCCFile"] = string.Format("Uploaded File: {0}", fname);

                return View("Merge", model);
            }

            ModelState.AddModelError("ClearedChecksError", "Please supply a file name.");
            return View("Merge", model);
        }

        
        [HttpPost]
        public ActionResult PerformMerge()
        {
            string uploadedFile = TempData["UploadedFile"] as string;
            string fileType = TempData["FileType"] as string;

            if (string.IsNullOrEmpty(uploadedFile))
            {
                ViewData["MergeStatus"] = "Please choose a file to merge";
                return View("Merge");
            }

            Merger.PerformMerge(uploadedFile, fileType);

            ViewData["MergeStatus"] = "Merge Complete";
            return View("Merge");
        }
    }
}
