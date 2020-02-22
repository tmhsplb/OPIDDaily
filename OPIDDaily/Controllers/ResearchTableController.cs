using OPIDDaily.DataContexts;
using OpidDailyEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using DataTables.Mvc;
using OPIDDaily.DAL;
using OPIDDaily.Models;

namespace OPIDDaily.Controllers
{
 
    public class ResearchTableController : Controller
    {
        public ActionResult ResearchTable()
        {
            return View();
        }

        // Don't know how to move this to the CheckManager where it belongs because of return type issues.
        // Just leave it here for now.
        public JsonResult GetChecks([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                IQueryable<RCheck> query = opidcontext.RChecks;

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

                var data = fqop.Select(rcheck => new
                {
                    sRecordID = rcheck.sRecordID,
                    sInterviewRecordID = rcheck.sInterviewRecordID,
                    Name = rcheck.Name,
                    sDOB = rcheck.sDOB,
                    sNum = rcheck.sNum,
                    sDate = rcheck.sDate,
                    Service = rcheck.Service,
                    Disposition = rcheck.Disposition
                }).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, filteredCount, totalCount), JsonRequestBehavior.AllowGet);
            }
        }

        

        [HttpPost]
        public ActionResult DeleteResearchTable()
        {
            CheckManager.DeleteResearchTable();
            return View("ResearchTable");
        }

        [HttpPost]
        public ActionResult RestoreResearchTable(FileViewModel model)
        {
            if (!CheckManager.ResearchTableIsEmpty())
            {
                ModelState.AddModelError("", "The Research Table must be empty before a restore is performed.");
                return View("ResearchTable", model);
            }

            if (ModelState.IsValid)
            {
                var postedFile = Request.Files["File"];

                if (!postedFile.FileName.EndsWith("xlsx"))
                {
                    ModelState.AddModelError("", "This is not an Excel xlsx file.");
                    return View("ResearchTable", model);
                }

                List<string> docfiles = FileUploader.UploadFile(postedFile);

                CheckManager.RestoreResearchTable(postedFile.FileName);

                return View("ResearchTable", model);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteAncientResearchChecks(FileViewModel model)
        {
            CheckManager.DeleteAncientResearchChecks(Convert.ToInt32(model.Year));
            ViewData["DeletedAncientChecks"] = string.Format("Deleted checks for year {0}", model.Year);
            return View("ResearchTable");
        }
    }
}