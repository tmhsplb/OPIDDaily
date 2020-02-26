using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDDaily.Utils
{
    public class Extras
    {
        public static DateTime DateTimeToday ()
        {
            // This is tricky!
            // See: https://stackoverflow.com/questions/14576967/datetime-today-vs-datetime-now
            DateTime now = DateTimeNow();
            return now.Date;
        }

        public static DateTime DateTimeNow()
        {
            // This compensates for the fact that DateTime.Now on the AppHarbor server returns
            // the time in the timezone of the server.
            // Here we convert UTC to Central Standard Time to get the time in Houston.
            // It also properly handles daylight savings time.
            DateTime now = DateTime.Now.ToUniversalTime();
            DateTime cstNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(now, "UTC", "Central Standard Time");
            return cstNow;
        }

        public static string GetTimestamp()
        {
            // Set timestamp when resolvedController is loaded. This allows
            // the timestamp to be made part of the page title, which allows
            // the timestamp to appear in the printed file and also as part
            // of the Excel file name of both the angular datatable and
            // the importme file.

            // This compensates for the fact that DateTime.Now on the AppHarbor server returns
            // the time in the timezone of the server.
            // Here we convert UTC to Central Standard Time to get the time in Houston.
            // It also properly handles daylight savings time.
            DateTime now = DateTime.Now.ToUniversalTime();
            DateTime cst = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(now, "UTC", "Central Standard Time");
            string timestamp = cst.ToString("dd-MMM-yyyy-hhmm");

            return timestamp;
        }

        public static string GetResearchTableFileName()
        {
            string timestamp = GetTimestamp();
            string fname = string.Format("Research {0}", timestamp);
            return fname;
        }

        public static string GetAncientChecksFileName(int year)
        {
            string timestamp = GetTimestamp();
            string fname = string.Format("AncientChecks {0} {1}", year, timestamp);
            return fname;
        }

        public static string GetImportMeFileName()
        {
            string timestamp = GetTimestamp();
            string fname = string.Format("interview-importme-{0}", timestamp);
            return fname;
        }

        public static string StripSuffix(string lastName)
        {
            // Examples:
            //   lastName = "Crow" --> prefix = "Crow"
            //   lastName = "Crow " --> prefix = "Crow"
            //   lastName = " Crow" --> prefix = "Crow"
            //   lastName = "Crow, Jr." --> prefix = "Crow"
            //   lastName = "Crow , Jr" --> prefix = "Crow"
            //   lastName = "Crow III" --> prefix = "Crow"
            lastName = lastName.Trim();
            string[] parts = lastName.Split(' ');
            string[] firstParts = parts[0].Split(',');
            string prefix = firstParts[0].Trim();
            return prefix;
        }

    }
}