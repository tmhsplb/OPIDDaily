using Microsoft.AspNet.Identity.EntityFramework;
using OPIDDaily.DataContexts;
using OPIDDaily.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpidDailyEntities;
 
namespace OPIDDaily.DAL
{
    public class DataManager
    {
        public static string EditUser(InvitationViewModel invite)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Invitation invitation = opiddailycontext.Invitations.Find(invite.Id);

                if (invitation != null && invitation.Accepted != (System.DateTime)System.Data.SqlTypes.SqlDateTime.Null)
                {
                    return "Registered";
                }

                invitation.UserName = invite.UserName;
                invitation.FullName = invite.FullName;
                invitation.Email = invite.Email;
                invitation.Role = invite.Role;
                
                opiddailycontext.SaveChanges();
                return "Success";
            }
        }
    }
}