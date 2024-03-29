﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace OPIDDaily
{
    public class Config
    {
        public static int CaseManagerExpiryDuration
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["CaseManagerExpiryDuration"]);
            }
        }
        public static string ConnectionString
        {
            get
            {
                // The value of OpidDailyConnectionString configured on Web.config is overwritten at AppHarbor deployment time.
                return ConfigurationManager.ConnectionStrings["HereToServeConnectionString"].ToString();
            }
        }

        public static string TrainingClients
        {
            get
            {
                return ConfigurationManager.AppSettings["TrainingClients"];
            }
        }

        public static string WorkingDesktopConnectionString
        {
            get
            {
               // return ConfigurationManager.AppSettings["SQLSERVER_CONNECTION_STRING"];
                return "data source=DESKTOP-0U83VML\\SqlExpress;initial catalog=OpidDailyDB;Integrated Security=True";
            }
        }

        public static string WorkingTrainingConnectionString
        {
            get
            {
                return "Server=7bcbd8b7-5eb2-4fd5-9046-abb401405c20.sqlserver.sequelizer.com;Database=db7bcbd8b75eb24fd59046abb401405c20;User ID=gosvlqzhyvsoquuf;Password=GJMBHbiCMyNcCG2ib3mScPk2zmEAxYJtn3HPqN7ozpkb73Wx8JFSJfj2yTbvCPfW;";
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

        public static string RecentYears
        {
            get
            {
                return ConfigurationManager.AppSettings["RecentYears"];
            }
        }

        public static string AncientYears
        {
            get
            {
                return ConfigurationManager.AppSettings["AncientYears"];
            }
        }
    }
}