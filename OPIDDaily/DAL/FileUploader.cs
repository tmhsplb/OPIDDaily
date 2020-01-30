using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDDaily.DAL
{
    public class FileUploader
    {
        public static List<string> UploadFile(HttpPostedFileBase postedFile)
        {
            List<string> docfiles = new List<string>();
            string filePath = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/Uploads/{0}", postedFile.FileName));
            postedFile.SaveAs(filePath);

            docfiles.Add(filePath);

            return docfiles;
        }
    }
}