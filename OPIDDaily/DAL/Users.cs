using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OPIDDaily.DataContexts;
using OPIDDaily.Models;
using OPIDDaily.Utils;
using OpidDailyEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDDaily.DAL
{
    public class Users
    {
        public static void SetDashboardMsgCnt()
        {
            DateTime today = Extras.DateTimeToday();
            int msgCnt = 0;

            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                List<Client> clients = opiddailycontext.Clients.Where(c => c.HHId == null && c.ServiceDate != c.Expiry && today <= c.Expiry && c.IsActive == true).ToList();

                foreach (Client client in clients)
                {
                    if (!string.IsNullOrEmpty(client.Msgs) && !client.Msgs.StartsWith("END"))
                    {
                        msgCnt += 1;
                    }
                }

                // Log.Info(string.Format("MsgCnt = {0}", msgCnt));

                SessionHelper.Set("MsgCnt", msgCnt.ToString());
            }
        }

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

        public static void EditUserAgencyId(int oldAgencyId, int newAgencyId)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                List<Invitation> invitations = opiddailycontext.Invitations.Where(i => i.AgencyId == oldAgencyId).ToList();

                using (IdentityDB identitycontext = new IdentityDB())
                {
                    var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(identitycontext));
                   
                    foreach (Invitation invite in invitations)
                    {
                        // The UserName in the Invitations table is the same as the UserName in table AspNetUsers. 
                        ApplicationUser user = UserManager.FindByName(invite.UserName);
                        user.AgencyId = newAgencyId;
                        UserManager.Update(user);
                        invite.AgencyId = newAgencyId;
                    }
                }

                opiddailycontext.SaveChanges();
            }
        }
    }
}