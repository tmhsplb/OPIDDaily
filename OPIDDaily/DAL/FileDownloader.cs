using OPIDDaily.Models;
using OPIDDaily.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
//using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace OPIDDaily.DAL
{
    public class FileDownloader
    {
        public static string DownloadResearchTable()
        {
            List<CheckViewModel> checks = CheckManager.GetChecks();

            if (checks != null)
            {
                string fname = Extras.GetResearchTableFileName();
                string pathToResearchTableFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/Downloads/{0}.csv", fname));

                WriteChecksFile(pathToResearchTableFile, checks);
                return fname;
            }

            return "NoChecks";
        }

        /*
        public static string DownloadAncientChecks(int year)
        {
            List<CheckViewModel> checks = CheckManager.GetAncientChecks(year);

            if (checks != null)
            {
                string fname = Extras.GetAncientChecksFileName(year);
                string pathToAncientHistoryFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/Downloads/{0}.csv", fname));

                WriteChecksFile(pathToAncientHistoryFile, checks);
                return fname;
            }

            return "NoChecks";
        }
        */
        public static string GetContentAsString(string fname)
        {

            string filePath = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/Downloads/{0}.csv", fname));

            Byte[] bytes = null;
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            bytes = br.ReadBytes((Int32)fs.Length);
            br.Close();
            fs.Close();

            System.IO.MemoryStream stream = new MemoryStream(bytes);
            string content = Encoding.ASCII.GetString(stream.ToArray());
            return content;
        }

        public static void WriteChecksFile(string pathToChecksFile, List<CheckViewModel> checks)
        {
            var csv = new StringBuilder();

            // N.B. No spaces between column names in the header row!
            string header = "Date,Record ID,Interview Record ID,Name,DOB,Check Number,Service,Disposition";
            csv.AppendLine(header);

            foreach (CheckViewModel check in checks)
            {
                string csvrow = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                    check.Date,
                    check.RecordID,
                    check.InterviewRecordID,
                    string.Format("\"{0}\"", check.Name),
                    check.DOB.ToString("MM/dd/yyyy"),
                    check.Num,
                    check.Service,
                    check.Disposition);

                csv.AppendLine(csvrow);
            }

            File.WriteAllText(pathToChecksFile, csv.ToString());
        }
    }
}