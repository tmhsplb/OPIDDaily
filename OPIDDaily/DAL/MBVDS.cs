using OPIDDaily.DataContexts;
using OPIDDaily.Models;
using OpidDailyEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPIDDaily.DAL
{
    public class MBVDS
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger(typeof(Agencies));

        public static SelectList GetMBVDSelectList()
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                List<MBVD> mbvds = opiddailycontext.MBVDS.ToList();

                mbvds = mbvds.OrderBy(m => m.MBVDId).ToList();
 
                SelectList sl = new SelectList(mbvds, "MBVDId", "MBVDName");

                return sl;
            }
        }

        public static List<MBVDViewModel> GetMBVDS()
        {
            List<MBVDViewModel> mbvdvms = new List<MBVDViewModel>();

            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                List<MBVD> mbvds = opiddailycontext.MBVDS.ToList();

                mbvds = mbvds.OrderBy(m => m.MBVDId).ToList();

                foreach (MBVD mbvd in mbvds)
                {
                    mbvdvms.Add(MBVDToMBVDViewModel(mbvd));
                }

                return mbvdvms;
            }
        }

        public static string GetMBVDName(int mbvdId)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                MBVD mbvd = opiddailycontext.MBVDS.Where(m => m.MBVDId == mbvdId).SingleOrDefault();

                if (mbvd != null)
                {
                    return mbvd.MBVDName;
                }

                return "unknown mbvd";   
            }
        }
        

        private static MBVD MBVDViewModelToMBVD(MBVDViewModel mbvdvm)
        {
            return new MBVD
            {
                MBVDId = mbvdvm.MBVDId,
                MBVDName = mbvdvm.MBVDName,
                IsActive = (mbvdvm.IsActive == "Yes" ? true : false)
            };
        }

        private static MBVDViewModel MBVDToMBVDViewModel(MBVD mbvd)
        {
            return new MBVDViewModel
            {
                Id = mbvd.Id,
                MBVDId = mbvd.MBVDId,
                MBVDName = mbvd.MBVDName,
                IsActive = (mbvd.IsActive ? "Yes" : string.Empty)
            };
        }

        public static void AddMBVD(MBVDViewModel mbvdvm)
        {
            MBVD mbvd = MBVDViewModelToMBVD(mbvdvm);

            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                opiddailycontext.MBVDS.Add(mbvd);
                opiddailycontext.SaveChanges();
            }
        }

        public static void EditMBVD(MBVDViewModel mbvdvm)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                MBVD mbvd = opiddailycontext.MBVDS.Find(mbvdvm.Id);

                if (mbvd != null)
                {
                    mbvd.MBVDId = mbvdvm.MBVDId;
                    mbvd.MBVDName = mbvdvm.MBVDName;
                    mbvd.IsActive = (mbvdvm.IsActive == "Yes" ? true : false);
                    opiddailycontext.SaveChanges();
                }
            }
        }

        public static void DeleteMBVD(MBVDViewModel mbvdvm)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                MBVD mbvd = opiddailycontext.MBVDS.Find(mbvdvm.MBVDId);
                opiddailycontext.MBVDS.Remove(mbvd);
                opiddailycontext.SaveChanges();
            }
        }
    }
}