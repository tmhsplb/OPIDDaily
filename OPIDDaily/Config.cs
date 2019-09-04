using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace OPIDDaily
{
    public class Config
    {
        public static string ConnectionString
        {
            get
            {
                // The value configured on Web.config is overwritten at AppHarbor deployment time.
                //return "Data Source=DEKTOP-GDDTDIC\\SqlExpress;Initial Catalog=OpidDB;Integrated Security=True";
                return ConfigurationManager.AppSettings["SQLSERVER_CONNECTION_STRING"];
            }
        }

        public static string WorkingDesktopConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["SQLSERVER_CONNECTION_STRING"];
            }
        }

        public static string WorkingStagingConnectionString
        {
            get
            {
                return "Server=729e1dc9-4b8c-486e-9d65-aabe015accd3.sqlserver.sequelizer.com;Database=db729e1dc94b8c486e9d65aabe015accd3;User ID=yadsgyjugioocuyw;Password=yTxBdAhtthhVyucQw58T8rRGSwqNetdrznZyhTHF2VyhYGy4rZmiWm2gcXRPjBoa;";
            }
        }

        public static string WorkingProductionConnectionString
        {
            get
            {
                return "Server=57d3c1fb-7e27-4b81-a9f2-aa8f0182310e.sqlserver.sequelizer.com;Database=db57d3c1fb7e274b81a9f2aa8f0182310e;User ID=fnalfcnlfsgdzgys;Password=LBwjPwsBZk577fNWh25JqJh3AKBaQiQzibLomTwYoGJckFBgdz8dswBcuCRd3sTQ;";
            }
        }

        public static string SuperadminEmail
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["SuperadminEmail"];
            }
        }

        public static string SuperadminPassword
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["SuperadminPwd"];
            }
        }

        public static string Release
        {
            get
            {
                return ConfigurationManager.AppSettings["Release"];
            }
        }
    }
}