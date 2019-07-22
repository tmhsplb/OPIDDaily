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

    }
}