using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDDaily.DAL
{
    public class SessionHelper
    {
        public static void Set(string key, string value)
        {
            HttpContext.Current.Session.Add(key, value);
        }

        public static string Get(string key)
        {
            Object val = HttpContext.Current.Session[key];

            if (val == null)
            {
                return "0";
            }

            return val.ToString();
        }
    }
}