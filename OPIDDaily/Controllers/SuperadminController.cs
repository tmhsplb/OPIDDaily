using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using DataTables.Mvc;
using OpidDaily.Models;
using OPIDDaily.DAL;
using OPIDDaily.DataContexts;
using OPIDDaily.Models;
using OPIDDaily.Utils;
using OpidDailyEntities;

namespace OPIDDaily.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SuperadminController : SharedController 
    {
        public ActionResult Home()
        {
            string workingConnectionString = string.Empty;

            ViewBag.Release = Config.GetRelease();  /* was Config.Release */
             
            switch (Config.GetRelease())  /* was Config.Release */
            {
                case "Desktop":
                    workingConnectionString = Config.WorkingDesktopConnectionString;
                    break;
                case "Staging":
                    workingConnectionString = Config.WorkingStagingConnectionString;
                    break;
                case "Production":
                    workingConnectionString = Config.WorkingProductionConnectionString;
                    break;
            }

            ViewBag.ConnectionString = Config.ConnectionString;
            ViewBag.ChangedConnectionString = (Config.ConnectionString.Equals(workingConnectionString) ? "False" : "True");

            // Log.Info("Goto Superadmin home page");
            return View();
        }

        public ActionResult ManageServiceDateClients()
        {
            string serviceDate = SessionHelper.Get("ServiceDate");
            if (serviceDate.Equals("0"))
            {
                DateTime today = Extras.DateTimeToday();
                serviceDate = today.ToString("ddd MMM d");
            }
            ViewBag.ServiceDate = DateTime.Parse(serviceDate).ToString("ddd MMM d");
            SessionHelper.Set("ServiceDate", serviceDate);
            return View("Clients");
        }

        public JsonResult GetServiceDateClients(int page, int rows)
        {
            string serviceDate = SessionHelper.Get("ServiceDate");
            DateTime selectedDate = DateTime.Parse(serviceDate);
            List<ClientViewModel> clients = Clients.GetClients(selectedDate, true, false);

            int pageIndex = page - 1;
            int pageSize = rows;
            int totalRecords = clients.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            clients = clients.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = clients
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
         
        public ActionResult ServiceDateReview()
        {
            string serviceDate = SessionHelper.Get("ServiceDate");
            if (serviceDate.Equals("0"))
            {
                DateTime today = Extras.DateTimeToday();
                serviceDate = today.ToString("ddd MMM d");
            }

            ViewBag.ServiceDate = DateTime.Parse(serviceDate).ToString("ddd MMM d");
            SessionHelper.Set("ServiceDate", serviceDate);
            return View("Review");
        }

        public JsonResult GetServiceDateReviewClients(int page, int rows)
        {
            string serviceDate = SessionHelper.Get("ServiceDate");
            DateTime selectedDate = DateTime.Parse(serviceDate);
            
            List<ClientReviewViewModel> clients = Clients.GetReviewClients(selectedDate);

            int pageIndex = page - 1;
            int pageSize = rows;
            int totalRecords = clients.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            clients = clients.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            clients = clients.OrderBy(c => c.CheckedIn).ToList();

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = clients
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeServiceDate(DatePickerViewModel dpvm)
        {
            SessionHelper.Set("ServiceDate", dpvm.datepicker);
            return RedirectToAction("ManageServiceDateClients");
        }

        public ActionResult PrepareServiceDateTable()
        {
            string serviceDate = SessionHelper.Get("ServiceDate");
            ViewBag.ServiceDate = DateTime.Parse(serviceDate).ToString("ddd MMM d, yyyy");

            DateTime specifiedDate = DateTime.Parse(serviceDate);

            List<ClientServedViewModel> clientsServed = Clients.ClientsServed(specifiedDate);
            return View("ClientsServed", clientsServed);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveServiceDateClients()
        {
            string serviceDate = SessionHelper.Get("ServiceDate");
            DateTime date = DateTime.Parse(serviceDate);
            Clients.RemoveClients(date);
            return RedirectToAction("ManageServiceDateClients");
        }

        public ActionResult Rebuild()
        {
            TempData["UploadedFile"] = "";
            return View();
        }

        public ActionResult AncientChecks()
        {
            return View();
        }

        // Don't know how to move this to the CheckManager where it belongs because of return type issues.
        // Just leave it here for now.
        public JsonResult GetAncientChecks([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                IQueryable<AncientCheck> query = opidcontext.AncientChecks;

                var totalCount = query.Count();

                // Apply filters for searching
                if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.Trim();
                    query = query.Where(p => p.Name.Contains(value) ||
                                             p.sDOB.Contains(value) ||
                                             p.sRecordID.Contains(value) ||
                                             p.sInterviewRecordID.Contains(value) ||
                                             p.sNum.Contains(value) ||
                                             p.sDate.Contains(value) ||
                                             p.Service.Contains(value) ||
                                             p.Disposition.Contains(value)
                                       );
                }

                var filteredCount = query.Count();

                var fqo = query.OrderBy("Id asc");  // Order by the primary key for speed. Ordering by Name times out, because Name is not an indexed field.

                // Paging
                var fqop = fqo.Skip(requestModel.Start).Take(requestModel.Length);

                var data = fqop.Select(ancientCheck => new
                {
                    sRecordID = ancientCheck.sRecordID,
                    sInterviewRecordID = ancientCheck.sInterviewRecordID,
                    Name = ancientCheck.Name,
                    sDOB = ancientCheck.sDOB,
                    sNum = ancientCheck.sNum,
                    sDate = ancientCheck.sDate,
                    Service = ancientCheck.Service,
                    Disposition = ancientCheck.Disposition
                }).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, filteredCount, totalCount), JsonRequestBehavior.AllowGet);
            }
        }

        /*
        [HttpPost]
        public ActionResult InsertAncientChecks(FileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var postedFile = Request.Files["File"];

                if (!postedFile.FileName.EndsWith("xlsx"))
                {
                    ModelState.AddModelError("", "This is not an Excel xlsx file.");
                    return View("AncientChecks", model);
                }

                List<string> docfiles = FileUploader.UploadFile(postedFile);

                CheckManager.InsertAncientChecks(postedFile.FileName);
                ViewData["InsertedAncientChecks"] = string.Format("Inserted File: {0}", postedFile.FileName);

                return View("AncientChecks", model);
            }

            return View("AncientChecks", model);
        }
        */
        
        [HttpPost]
        public ActionResult MergeBoundedResearchTableFile(FileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var postedFile = Request.Files["File"];

                string fname = postedFile.FileName;

                if (!fname.EndsWith("xlsx"))
                {
                    ModelState.AddModelError("BoundedResearchTableFileError", "This is not an Excel xlsx file.");
                    return View("Rebuild", model);
                }

                List<string> docfiles = FileUploader.UploadFile(postedFile);
                string pathToBoundedResearchFile = string.Format("~/Uploads/{0}", fname);
                string mappedPath = HttpContext.Server.MapPath(pathToBoundedResearchFile);

                Merger.PerformMerge(mappedPath, "BoundedResearchTableFile");

                ViewData["MergedBoundedResearchTableFile"] = string.Format("Merged File: {0}", fname);
                return View("Rebuild", model);
            }

            ModelState.AddModelError("BoundedResearchTableFileError", "Please supply a file name.");
            return View("Rebuild", model);
        }

        [HttpPost]
        public ActionResult MergeAncientChecksFile(FileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var postedFile = Request.Files["File"];

                string fname = postedFile.FileName;

                if (!fname.EndsWith("xlsx"))
                {
                    ModelState.AddModelError("AncientChecksFileError", "This is not an Excel xlsx file.");
                    return View("Rebuild", model);
                }

                List<string> docfiles = FileUploader.UploadFile(postedFile);
                string pathToAncientChecksFile = string.Format("~/Uploads/{0}", fname);
                string mappedPath = HttpContext.Server.MapPath(pathToAncientChecksFile);

                Merger.PerformMerge(mappedPath, "AncientChecksFile");

                ViewData["MergedAncientChecksFile"] = string.Format("Merged File: {0}", fname);
                return View("Rebuild", model);
            }

            ModelState.AddModelError("AncientChecksFileError", "Please supply a file name.");
            return View("Rebuild", model);
        }

        [HttpPost]
        public ActionResult DeleteAncientAncientChecks(FileViewModel model)
        {
            CheckManager.DeleteAncientAncientChecks(Convert.ToInt32(model.Year));
            ViewData["DeletedAncientAncientChecks"] = string.Format("Deleted checks for year {0}", model.Year);
            return View("AncientChecks");
        }

        public ActionResult Version()
        {
            return View("Version");
        }
    }
}