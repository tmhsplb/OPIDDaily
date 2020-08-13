using OPIDDaily.Models;
using System;
//using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using OPIDDaily.Controllers;

namespace OPIDDaily.Utils
{
    public class MyExcelDataReader
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger(typeof(MyExcelDataReader));
     
        private static CheckViewModel NewCheckViewModel(System.Data.DataRow dataRow)
        {
            try
            {
                CheckViewModel cvm = new CheckViewModel
                {
                    Date = DBNull.Value.Equals(dataRow["Date"]) ? string.Empty : dataRow["Date"].ToString(),
                    sDate = Convert.ToDateTime(dataRow["Date"].ToString()).ToString("MM/dd/yyyy"),
                    RecordID = Convert.ToInt32(dataRow["Record ID"].ToString()),
                    sRecordID = dataRow["Record ID"].ToString(),
                    InterviewRecordID = Convert.ToInt32(dataRow["Interview Record ID"].ToString()),
                    sInterviewRecordID = dataRow["Interview Record ID"].ToString(),
                    Name = dataRow["Name"].ToString(),
                    DOB = Convert.ToDateTime(dataRow["DOB"]),
                    Num = Convert.ToInt32(dataRow["Check Number"].ToString()),
                    sNum = dataRow["Check Number"].ToString(),
                    Service = dataRow["Service"].ToString(),
                    Disposition = dataRow["Disposition"].ToString()
                };

                return cvm;
            }
            catch (Exception e)
            {
                // log the dataRow that failed
                Log.Error(string.Format("NewCheckViewModel: {0}", e.Message));
                return null;
            }
        }

        public static List<CheckViewModel> GetCVMS(string filePath)
        {      
            List<CheckViewModel> rchecks = new ExcelData(filePath).GetData().Select(dataRow => NewCheckViewModel(dataRow)).ToList();
            return rchecks;
        }

        private static void InsertNulls(List<DispositionRow> resRows)
        {
            DateTime epochAsDateTime = new DateTime(1900, 1, 1);
            DateTime dt;
            DispositionRow rr = new DispositionRow(); ;

            try
            {
                foreach (DispositionRow dr in resRows)
                {
                    if (dr != null) // NewDispositionRow may have found a corrupt record and inserted a null
                    {
                        rr = dr;

                        dt = (DateTime)dr.LBVDOrderDateTwo;
                        if (dt.CompareTo(epochAsDateTime) == 0)
                        {
                            dr.LBVDOrderDateTwo = null;
                        }

                        dt = (DateTime)dr.LBVDOrderDateThree;
                        if (dt.CompareTo(epochAsDateTime) == 0)
                        {
                            dr.LBVDOrderDateThree = null;
                        }

                        dt = (DateTime)dr.MBVDOrderDateTwo;
                        if (dt.CompareTo(epochAsDateTime) == 0)
                        {
                            dr.MBVDOrderDateTwo = null;
                        }

                        dt = (DateTime)dr.MBVDOrderDateThree;
                        if (dt.CompareTo(epochAsDateTime) == 0)
                        {
                            dr.MBVDOrderDateThree = null;
                        }

                        dt = (DateTime)dr.TIDOrderDateTwo;
                        if (dt.CompareTo(epochAsDateTime) == 0)
                        {
                            dr.TIDOrderDateTwo = null;
                        }

                        dt = (DateTime)dr.TIDOrderDateThree;
                        if (dt.CompareTo(epochAsDateTime) == 0)
                        {
                            dr.TIDOrderDateThree = null;
                        }

                        dt = (DateTime)dr.TDLOrderDateTwo;
                        if (dt.CompareTo(epochAsDateTime) == 0)
                        {
                            dr.TDLOrderDateTwo = null;
                        }

                        dt = (DateTime)dr.TDLOrderDateThree;
                        if (dt.CompareTo(epochAsDateTime) == 0)
                        {
                            dr.TDLOrderDateThree = null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(string.Format("InsertNulls: RecordID = {0}, InterviewRecordID = {1} Name = {2}, {3}", rr.RecordID, rr.InterviewRecordID, rr.Lname, rr.Fname));
                Log.Error(e.Message);
            }
        }

        private static ClientRow NewClientRow(System.Data.DataRow dataRow)
        {
            string lname = string.Empty, fname = string.Empty, mname = string.Empty, birthName = string.Empty;
            int recordID = 0;
            DateTime dob = Extras.DateTimeToday();

            try
            {
                recordID = Convert.ToInt32(dataRow["Record ID"].ToString());
                lname = dataRow["Last Name"].ToString();
                fname = dataRow["First Name"].ToString();
                mname = dataRow["Middle Name"].ToString();
                birthName = dataRow["Birth Name"].ToString();
                dob = Convert.ToDateTime(dataRow["DOB"].ToString());

                ClientRow cr = new ClientRow
                {
                    RecordID = recordID,
                    LastName = lname,
                    FirstName = fname,
                    MiddleName = mname,
                    BirthName = birthName,
                    DOB = dob
                };

                return cr;
            }
            catch (Exception e)
            {
                // log the dataRow that failed
                Log.Warn(string.Format("Bad new client record: RecordID = {0}, Name = {1}, {2}, DOB = {3}", recordID, lname, fname, dob));
                Log.Error(e.Message);
                return null;
            }
        }

        private static DispositionRow NewDispositionRow(System.Data.DataRow dataRow, string epoch)
        {
            string interviewDate = string.Empty, lname = string.Empty, fname = string.Empty;
            int recordID = 0, interviewRecordID = 0;
            DateTime dob = Extras.DateTimeToday();

            try
            {
                recordID = Convert.ToInt32(dataRow["Record ID"].ToString());
                interviewRecordID = Convert.ToInt32(dataRow["Interview Record ID"].ToString());
                interviewDate = dataRow["OPID Interview Date"].ToString();
                lname = dataRow["Last Name"].ToString();
                fname = dataRow["First Name"].ToString();
                dob = Convert.ToDateTime(dataRow["Date of Birth"].ToString());

                if (string.IsNullOrEmpty(interviewDate))
                {
                    Log.Warn(string.Format("Bad record (1): RecordID = {0}, InterviewRecordID = {1}, Name = {2}, {3}, DOB = {4}", recordID, interviewRecordID, lname, fname, dob));
                    return null;
                }

                DispositionRow dr = new DispositionRow
                {
                    RecordID = recordID,
                    InterviewRecordID = interviewRecordID,
                    Lname = lname,
                    Fname = fname,
                    DOB = dob,
                    Date = Convert.ToDateTime(interviewDate),
                    RequestedItem = dataRow["Requested Item"].ToString(),

                    LBVDCheckNum = Convert.ToInt32(dataRow["LBVD Check Number"].ToString()),
                    LBVDCheckDisposition = dataRow["LBVD Check Disposition"].ToString(),

                    LBVDOrderDateTwo = DBNull.Value.Equals(dataRow["LBVD Order Date Two"]) ? Convert.ToDateTime(epoch) : Convert.ToDateTime(dataRow["LBVD Order Date Two"].ToString()),
                    LBVDCheckNum2 = Convert.ToInt32(dataRow["LBVD Check Number Two"].ToString()),
                    LBVDCheck2Disposition = dataRow["LBVD Check Two Disposition"].ToString(),

                    LBVDOrderDateThree = DBNull.Value.Equals(dataRow["LBVD Order Date Three"]) ? Convert.ToDateTime(epoch) : Convert.ToDateTime(dataRow["LBVD Order Date Three"].ToString()),
                    LBVDCheckNum3 = Convert.ToInt32(dataRow["LBVD Check Number Three"].ToString()),
                    LBVDCheck3Disposition = dataRow["LBVD Check Three Disposition"].ToString(),

                    TIDCheckNum = Convert.ToInt32(dataRow["TID Check Number"].ToString()),
                    TIDCheckDisposition = dataRow["TID Check Disposition"].ToString(),

                    TIDOrderDateTwo = DBNull.Value.Equals(dataRow["TID Order Date Two"]) ? Convert.ToDateTime(epoch) : Convert.ToDateTime(dataRow["TID Order Date Two"].ToString()),
                    TIDCheckNum2 = Convert.ToInt32(dataRow["TID Check Number Two"].ToString()),
                    TIDCheck2Disposition = dataRow["TID Check Two Disposition"].ToString(),

                    TIDOrderDateThree = DBNull.Value.Equals(dataRow["TID Order Date Three"]) ? Convert.ToDateTime(epoch) : Convert.ToDateTime(dataRow["TID Order Date Three"].ToString()),
                    TIDCheckNum3 = Convert.ToInt32(dataRow["TID Check Number Three"].ToString()),
                    TIDCheck3Disposition = dataRow["TID Check Three Disposition"].ToString(),

                    TDLCheckNum = Convert.ToInt32(dataRow["TDL Check Number"].ToString()),
                    TDLCheckDisposition = dataRow["TDL Check Disposition"].ToString(),

                    TDLOrderDateTwo = DBNull.Value.Equals(dataRow["TDL Order Date Two"]) ? Convert.ToDateTime(epoch) : Convert.ToDateTime(dataRow["TDL Order Date Two"].ToString()),
                    TDLCheckNum2 = Convert.ToInt32(dataRow["TDL Check Number Two"].ToString()),
                    TDLCheck2Disposition = dataRow["TDL Check Two Disposition"].ToString(),

                    TDLOrderDateThree = DBNull.Value.Equals(dataRow["TDL Order Date Three"]) ? Convert.ToDateTime(epoch) : Convert.ToDateTime(dataRow["TDL Order Date Three"].ToString()),
                    TDLCheckNum3 = Convert.ToInt32(dataRow["TDL Check Number Three"].ToString()),
                    TDLCheck3Disposition = dataRow["TDL Check Three Disposition"].ToString(),

                    MBVDCheckNum = Convert.ToInt32(dataRow["MBVD Check Number"].ToString()),
                    MBVDCheckDisposition = dataRow["MBVD Check Disposition"].ToString(),

                    MBVDOrderDateTwo = DBNull.Value.Equals(dataRow["MBVD Order Date Two"]) ? Convert.ToDateTime(epoch) : Convert.ToDateTime(dataRow["MBVD Order Date Two"].ToString()),
                    MBVDCheckNum2 = Convert.ToInt32(dataRow["MBVD Check Number Two"].ToString()),
                    MBVDCheck2Disposition = dataRow["MBVD Check Two Disposition"].ToString(),

                    MBVDOrderDateThree = DBNull.Value.Equals(dataRow["MBVD Order Date Three"]) ? Convert.ToDateTime(epoch) : Convert.ToDateTime(dataRow["MBVD Order Date Three"].ToString()),
                    MBVDCheckNum3 = Convert.ToInt32(dataRow["MBVD Check Number Three"].ToString()),
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
                };

                return dr;
            }
            catch (Exception e)
            {
                // log the dataRow that failed
                Log.Warn(string.Format("Bad record (2): RecordID = {0}, InterviewRecordID = {1}, Name = {2}, {3}, DOB = {4}", recordID, interviewRecordID, lname, fname, dob));
                Log.Error(e.Message);
                return null;
            }           
        }

        public static List<ClientRow> GetClientRows(string filePath)
        {
            List<ClientRow> clientRows = new ExcelData(filePath).GetData().Select(dataRow => NewClientRow(dataRow)).ToList();

           // InsertNulls(dispositionRows);
            return clientRows;
        }

        public static List<DispositionRow> GetResearchRows(string filePath)
        {
            string epoch = "01/01/1900"; // Use this in place of a null value, because I couldn't make null work
            List<DispositionRow> dispositionRows = new ExcelData(filePath).GetData().Select(dataRow => NewDispositionRow(dataRow, epoch)).ToList();

            InsertNulls(dispositionRows);
            return dispositionRows;
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
                Log.Error(string.Format("GetDateValue: {0}", e.Message));
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
                Log.Error(string.Format("GetCheckNum: {0}", e.Message));
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