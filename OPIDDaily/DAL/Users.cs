using OPIDDaily.DataContexts;
using OPIDDaily.Models;
using OpidDailyEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDDaily.DAL
{
    public class Users
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
                invitation.AgencyId = Convert.ToInt32(invite.AgencyId);

                opiddailycontext.SaveChanges();
                return "Success";
            }
        }
    }
}