using OPIDDaily.Models;
using System;
//using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDDaily.Utils
{
    public class MyExcelDataReader
    {
        public static List<CheckViewModel> GetCVMS(string filePath)
        {
            List<CheckViewModel> rchecks = new ExcelData(filePath).GetData().Select(dataRow => new CheckViewModel
            {
                Date = Convert.ToDateTime(dataRow["Date"].ToString()), //dataRow["Date"].ToString(),
                sDate = Convert.ToDateTime(dataRow["Date"].ToString()).ToString("MM/dd/yyyy"),
                RecordID = Convert.ToInt32(dataRow["Record ID"].ToString()),
                sRecordID = dataRow["Record ID"].ToString(),
                InterviewRecordID = Convert.ToInt32(dataRow["Interview Record ID"].ToString()),
                sInterviewRecordID = dataRow["Interview Record ID"].ToString(),
                Name = dataRow["Name"].ToString(),
                Num = Convert.ToInt32(dataRow["Check Number"].ToString()),
                sNum = dataRow["Check Number"].ToString(),
                Service = dataRow["Service"].ToString(),
                Disposition = dataRow["Disposition"].ToString()
            }).ToList();

            return rchecks;
        }

        public static List<DispositionRow> GetResearchRows(string filePath)
        { 
            try
            {
                List<DispositionRow> resRows = new ExcelData(filePath).GetData().Select(dataRow => new DispositionRow
                {
                    RecordID = Convert.ToInt32(dataRow["Record ID"].ToString()),
                    InterviewRecordID = Convert.ToInt32(dataRow["Interview Record ID"].ToString()),
                    Lname = dataRow["Last Name"].ToString(),
                    Fname = dataRow["First Name"].ToString(),
                    DOB = Convert.ToDateTime(dataRow["Date of Birth"].ToString()),
                    Date = Convert.ToDateTime(dataRow["OPID Interview Date"].ToString()),

                    LBVDCheckNum = Convert.ToInt32(dataRow["LBVD Check Number"].ToString()),
                    LBVDCheckDisposition = dataRow["LBVD Check Disposition"].ToString(),

                    LBVDCheckNum2 = Convert.ToInt32(dataRow["LBVD Check Number Two"].ToString()),
                    LBVDOrderDateTwo = DBNull.Value.Equals(dataRow["LBVD Order Date Two"]) ? (DateTime?)null : Convert.ToDateTime(dataRow["LBVD Order Date Two"].ToString()),
                    LBVDCheck2Disposition = dataRow["LBVD Check Two Disposition"].ToString(),
                    
                    LBVDCheckNum3 = Convert.ToInt32(dataRow["LBVD Check Number Three"].ToString()),
                    LBVDOrderDateThree = DBNull.Value.Equals(dataRow["LBVD Order Date Three"]) ? (DateTime?)null : Convert.ToDateTime(dataRow["LBVD Order Date Three"].ToString()),
                    LBVDCheck3Disposition = dataRow["LBVD Check Three Disposition"].ToString(),
                    
                    TIDCheckNum = Convert.ToInt32(dataRow["TID Check Number"].ToString()),
                    TIDCheckDisposition = dataRow["TID Check Disposition"].ToString(),

                    TIDCheckNum2 = Convert.ToInt32(dataRow["TID Check Number Two"].ToString()),
                    TIDOrderDateTwo = DBNull.Value.Equals(dataRow["TID Order Date Two"]) ? (DateTime?)null : Convert.ToDateTime(dataRow["TID Order Date Two"].ToString()),
                    TIDCheck2Disposition = dataRow["TID Check Two Disposition"].ToString(),

                    TIDCheckNum3 = Convert.ToInt32(dataRow["TID Check Number Three"].ToString()),
                    TIDOrderDateThree = DBNull.Value.Equals(dataRow["TID Order Date Three"]) ? (DateTime?)null : Convert.ToDateTime(dataRow["TID Order Date Three"].ToString()),
                    TIDCheck3Disposition = dataRow["TID Check Three Disposition"].ToString(),

                    TDLCheckNum = Convert.ToInt32(dataRow["TDL Check Number"].ToString()),
                    TDLCheckDisposition = dataRow["TDL Check Disposition"].ToString(),

                    TDLCheckNum2 = Convert.ToInt32(dataRow["TDL Check Number Two"].ToString()),
                    TDLOrderDateTwo = DBNull.Value.Equals(dataRow["TDL Order Date Two"]) ? (DateTime?)null : Convert.ToDateTime(dataRow["TDL Order Date Two"].ToString()),  
                    TDLCheck2Disposition = dataRow["TDL Check Two Disposition"].ToString(),

                    TDLCheckNum3 = Convert.ToInt32(dataRow["TDL Check Number Three"].ToString()),
                    TDLOrderDateThree = DBNull.Value.Equals(dataRow["TDL Order Date Three"]) ? (DateTime?)null : Convert.ToDateTime(dataRow["TDL Order Date Three"].ToString()),
                    TDLCheck3Disposition = dataRow["TDL Check Three Disposition"].ToString(),

                    MBVDCheckNum = Convert.ToInt32(dataRow["MBVD Check Number"].ToString()),
                    MBVDCheckDisposition = dataRow["MBVD Check Disposition"].ToString(),

                    MBVDCheckNum2 = Convert.ToInt32(dataRow["MBVD Check Number Two"].ToString()),
                    MBVDOrderDateTwo = DBNull.Value.Equals(dataRow["MBVD Order Date Two"]) ? (DateTime?)null : Convert.ToDateTime(dataRow["MBVD Order Date Two"].ToString()),
                    MBVDCheck2Disposition = dataRow["MBVD Check Two Disposition"].ToString(),

                    MBVDCheckNum3 = Convert.ToInt32(dataRow["MBVD Check Number Three"].ToString()),
                    MBVDOrderDateThree = DBNull.Value.Equals(dataRow["MBVD Order Date Three"]) ? (DateTime?)null : Convert.ToDateTime(dataRow["MBVD Order Date Three"].ToString()),
                    MBVDCheck3Disposition = dataRow["MBVD Check Three Disposition"].ToString()

                    // Supporting documents
                    /*
                    SDCheckNum1 = Convert.ToInt32(dataRow["SD Check Number 1"].ToString()),
                    SDCheckDisposition = dataRow["SD Check Disposition No 1"].ToString(),
                    SDCheckNum2 = Convert.ToInt32(dataRow["SD Check Number 2"].ToString()),
                    SDCheckDisposition2 = dataRow["SD Check Disposition No 2"].ToString(),
                    SDCheckNum3 = Convert.ToInt32(dataRow["SD Check Number 3"].ToString()),
                    SDCheckDisposition3 = dataRow["SD Check Disposition No 3"].ToString(),

                    SDOrderDate2 = DBNull.Value.Equals(dataRow["SD Order Date Two"]) ? (DateTime?)null : Convert.ToDateTime(dataRow["SD Order Date Two"].ToString()),
                    SDCheckNum12 = Convert.ToInt32(dataRow["SD Check Number 1, Two"].ToString()),
                    SDCheckDisposition12 = dataRow["SD Check Disposition No 1, Two"].ToString(),
                    SDCheckNum22 = Convert.ToInt32(dataRow["SD Check Number 2, Two"].ToString()),
                    SDCheckDisposition22 = dataRow["SD Check Disposition No 2, Two"].ToString(),
                    SDCheckNum32 = Convert.ToInt32(dataRow["SD Check Number 3, Two"].ToString()),
                    SDCheckDisposition32 = dataRow["SD Check Disposition No 3, Two"].ToString(),

                    SDOrderDate3 = DBNull.Value.Equals(dataRow["SD Order Date Three"]) ? (DateTime?)null : Convert.ToDateTime(dataRow["SD Order Date Three"].ToString()),
                    SDCheckNum13 = Convert.ToInt32(dataRow["SD Check Number 1, Three"].ToString()),
                    SDCheckDisposition13 = dataRow["SD Check Disposition No 1, Three"].ToString(),
                    SDCheckNum23 = Convert.ToInt32(dataRow["SD Check Number 2, Three"].ToString()),
                    SDCheckDisposition23 = dataRow["SD Check Disposition No 2, Three"].ToString(),
                    SDCheckNum33 = Convert.ToInt32(dataRow["SD Check Number 3, Three"].ToString()),
                    SDCheckDisposition33 = dataRow["SD Check Disposition No 3, Three"].ToString()
                    */
                }).ToList(); 

                return resRows;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private static DateTime GetDateValue(System.Data.DataRow row)
        {
            string dvalue;
            DateTime rdate = (DateTime)row["Date"];

            if (DBNull.Value.Equals(row["Date"]))  // For File1 and File2 read on Mach 30, 2018 
            {
                // This is a blank row. Provide a dummy value.
                dvalue = "12/12/1900";
            }
            else
            {
                dvalue = row["Date"].ToString();  // For File1 and File2 read on March 30, 2018
            }

            DateTime dtime = DateTime.Now;

            try
            {
                dtime = Convert.ToDateTime(dvalue);
            }
            catch (Exception e)
            {
                throw new Exception("Bad date value");
            }

            return dtime;
        }

        private static int GetCheckNum(System.Data.DataRow row)
        {
            string cvalue;

            if (DBNull.Value.Equals(row["Num"]))  // For File1 and File2 read on March 30, 2018
            {
                // This is a blank row. Provide a dummy value.
                cvalue = "0";
            }
            else
            {
                cvalue = row["Num"].ToString();  // For File1 and File2 read on March 30, 2018
                if (cvalue.Equals("EFT") || cvalue.Equals("Debit"))  // PLB 10/12/2017. Bill's file may have EFT or Debit in Num field. Treat as blank line.
                {
                    cvalue = "0";
                }
            }

            int cnum = 0;

            try
            {
                cnum = Convert.ToInt32(cvalue);
            }
            catch (Exception e)
            {
                throw new Exception("Bad number value");
            }

            return cnum;
        }

        public static List<Check> GetExcelChecks(string filePath)
        {
            List<Check> rowChecks = new ExcelData(filePath).GetData().Select(dataRow =>
                new Check
                {
                    Date = GetDateValue(dataRow),
                    Num = GetCheckNum(dataRow),
                }).ToList();

            List<Check> excelChecks = new List<Check>();

            // Remove checks corresponding to blank rows in Excel file.
            foreach (Check check in rowChecks)
            {
                if (check.Num != 0)
                {
                    excelChecks.Add(check);
                }
            }

            return excelChecks;
        }
    }
}