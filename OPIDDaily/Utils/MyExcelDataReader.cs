using OPIDDaily.Models;
using System;
//using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using OPIDDaily.Controllers;
using OpidDailyEntities;
using OPIDDaily.DAL;

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

        private static TrackingRow NewTrackingRow(System.Data.DataRow dataRow, string epoch)
        {
            string lname = dataRow["Last Name"].ToString();
            string fname = dataRow["First Name"].ToString();
            DateTime dob = Convert.ToDateTime(dataRow["Date of Birth"].ToString());
            string disp = string.Empty;

            try
            {
                int checkNumber = Convert.ToInt32(dataRow["Check Number"].ToString());
                string reissued = dataRow["Reissued"].ToString();
                string scammed = dataRow["Scammed"].ToString();
                int cid = Clients.IdentifyClient(lname, fname, dob);

                if (cid == 0)
                {
                    // This is the case where a check appears in the tracking file, but the client
                    // is not in the Clients table. The check may be a legacy 5-digit check in the research 
                    // table and should have its disposition changed according to what the tracking file says.
                    string disposition = dataRow["Check Disposition"].ToString();
                    CheckManager.SetDisposition(dataRow, checkNumber, disposition);
                    // Log.Warn(string.Format("Could not identify client: {0}, {1}, DOB = {2}", lname, fname, dob));
                    // Don't create a new tracking row.
                    return null;
                }

                string requestedItem = dataRow["Requested Item"].ToString();

                if (!string.IsNullOrEmpty(reissued) && (reissued.Equals("Reissued") || reissued.Equals("Replaced")))
                {
                    // We are reissuing an existing check. Change the disposition of
                    // the existing check to the reissuing reason.
                    // string reissuedReason = string.Format("{0}/{1}", reissued, dataRow["Reissued Reason"]).ToString();
                    // CheckManager.SetDisposition(dataRow, checkNumber, reissuedReason);

                    // Don't create a new tracking row.
                    // return null;
                    disp = string.Format("{0}/{1}", reissued, dataRow["Reissued Reason"]).ToString();
                }

                if (!string.IsNullOrEmpty(scammed) && scammed.Equals("Yes"))
                {
                    // We are marking an existing check as scammed. Change the disposition
                    // of the existing check to "Scammed Check"
                    //  CheckManager.SetDisposition(dataRow, checkNumber, "Scammed Check");

                    // Don't create a new tracking row.
                    //  return null;
                    disp = "Scammed Check";
                }

                List<VisitViewModel> visits = Visits.GetVisits(cid);
                requestedItem = CheckManager.SequencedRequestedItem(visits, requestedItem);
                return NewTrackingRow(requestedItem, lname, fname, dob, dataRow, epoch, disp);
            }
            catch (Exception e)
            {
                // log the dataRow that failed
                Log.Warn(string.Format("Bad tracking row:  Name = {0}, {1}, DOB = {2}", lname, fname, dob));
                Log.Error(e.Message);
                return null;
            }
        }

        private static TrackingRow NewTrackingRow(string requestedItem, string lname, string fname, DateTime dob, System.Data.DataRow dataRow, string epoch, string disp)
        {
            int recordID = Convert.ToInt32(dataRow["Record ID"].ToString());
            int interviewRecordID = Convert.ToInt32(dataRow["Interview Record ID"].ToString());
            DateTime epochDate = Convert.ToDateTime(epoch);

            try
            {
                string interviewDate = dataRow["OPID Interview Date"].ToString();
             //   DateTime orderDate = DBNull.Value.Equals(dataRow["Order Date"]) ? epochDate : Convert.ToDateTime(dataRow["Order Date"].ToString());
                string checkNumber = dataRow["Check Number"].ToString();
                string checkDisposition = (string.IsNullOrEmpty(disp) ? dataRow["Check Disposition"].ToString() : disp);

                if (string.IsNullOrEmpty(interviewDate))
                {
                    Log.Warn(string.Format("Bad record (1): RecordID = {0}, InterviewRecordID = {1}, Name = {2}, {3}, DOB = {4}", recordID, interviewRecordID, lname, fname, dob));
                    return null;
                }

                // The data entry person may have accidentally clicked the radio
                // button for Voided or Cleared. The way to fix this is to select
                // the radio button Mistakenly Marked. This should create a record
                // with no check disposition instead of one with dispositon Mistakenly Marked.
                if (checkDisposition.Equals("Mistakenly Marked"))
                {
                    checkDisposition = string.Empty;
                }

                TrackingRow tr = new TrackingRow
                {
                    RecordID = recordID,
                    InterviewRecordID = interviewRecordID,
                    Lname = lname,
                    Fname = fname,
                    DOB = dob,
                    Date = Convert.ToDateTime(interviewDate),
                //    OrderDate = orderDate,
                    RequestedItem = requestedItem,
                    CheckNum = Convert.ToInt32(checkNumber),
                    CheckDisposition = checkDisposition 
                };

                // Special test for tr.OrderDate
                /* PLB 10/08/2020 New interface does not use Order Date.
                if (orderDate.CompareTo(epochDate) == 0)
                {
                    tr.OrderDate = null;
                }
                */

                return tr;
            }
            catch (Exception e)
            {
                // log the dataRow that failed
                Log.Warn(string.Format("Bad record (2): RecordID = {0}, InterviewRecordID = {1}, Name = {2}, {3}, DOB = {4}", recordID, interviewRecordID, lname, fname, dob));
                Log.Error(string.Format("Above warning generated the following exception: {0}", e.Message));
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

        public static List<TrackingRow> GetTrackingRows(string filePath)
        {
            string epoch = "01/01/1900"; // Use this in place of a null value, because I couldn't make null work
            List<TrackingRow> trackingRows = new ExcelData(filePath).GetData().Select(dataRow => NewTrackingRow(dataRow, epoch)).ToList();

            return trackingRows;
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

        private static int GetMemo(System.Data.DataRow row, int num)
        {
            string memo;
            int interviewRecordID;

            if (DBNull.Value.Equals(row["Memo"]))  // For File1 and File2 read on March 30, 2018
            {
                // This is a blank field. Return 0.
                return 0;
            }

            try
            {
                memo = row["Memo"].ToString();
                interviewRecordID = Convert.ToInt32(memo);
            }
            catch (Exception e)
            {
                Log.Error(string.Format("Bad memo field for check num {0}", num));
                interviewRecordID = 0;
            }

            return interviewRecordID;
        }

        private static Check NewExcelCheck(System.Data.DataRow dataRow)
        {
            Check check;
            int num = GetCheckNum(dataRow);
            int interviewRecordID = GetMemo(dataRow, num);  // memo field might provide an Apricot tracking record id
           
            check = new Check
            {
                Date = GetDateValue(dataRow),
                Num = num,
                InterviewRecordID = interviewRecordID
            };

            return check;
        }

        public static List<Check> GetExcelChecks(string filePath)
        {
            List<Check> rowChecks = new ExcelData(filePath).GetData().Select(dataRow => NewExcelCheck(dataRow)).ToList();

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