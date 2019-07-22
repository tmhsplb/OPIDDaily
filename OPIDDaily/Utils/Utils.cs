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
            // This compensates for the fact that DateTime.Today on the AppHarbor server returns
            // the time in the timezone of the server.
            // Here we convert DateTime.Now.Date UTC to Central Standard Time to get today in Houston.
            // It also properly handles daylight savings time.
            DateTime today = DateTime.Now.Date.ToUniversalTime();
            DateTime cstToday = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(today, "UTC", "Central Standard Time");
            return cstToday;
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

    }
}