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
                return "Server=be4ae346-8a50-4d11-9322-a8f7013bf955.sqlserver.sequelizer.com;Database=dbbe4ae3468a504d119322a8f7013bf955;User ID=uuevjelrrnjlcmjb;Password=JkKEf63RCggWRnNvHTM7cZ8cDAiCgGg57DcgKnrY4zvpdN7BY82vXxaSkGaMWyJ4;";
            }
        }

        public static string WorkingProductionConnectionString
        {
            get
            {
                return "Server=6da6d9ef-7d51-442f-bfdc-a8f7014dca90.sqlserver.sequelizer.com;Database=db6da6d9ef7d51442fbfdca8f7014dca90;User ID=aeufensilubpmsyv;Password=BNLhPTfGCs4cnLcDYzp2XTrroE3buxoAJijhUBooepR2LQgXUFTdBfbRwt3iZenf;";
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