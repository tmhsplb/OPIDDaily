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
    public class Identity
    {
        private static RoleViewModel RoleEntityToRoleViewModel(IdentityRole idRole)
        {
            return new RoleViewModel
            {
                Id = idRole.Id,
                Name = idRole.Name
            };
        }

        public static List<RoleViewModel> GetRoles()
        {
            using (IdentityDB identitycontext = new DataContexts.IdentityDB())
            {
                List<RoleViewModel> roles = new List<RoleViewModel>();
                List<IdentityRole> identityRoles = identitycontext.Roles.ToList();

                foreach (IdentityRole idRole in identityRoles)
                {
                    roles.Add(RoleEntityToRoleViewModel(idRole));
                }

                return roles;
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
                invitation.Email = invite.Email;
                invitation.Role = invite.Role;

                opiddailycontext.SaveChanges();
                return "Success";
            }
        }

        public static Invitation AcceptedInvitation(string userName, string email)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Invitation invite = opiddailycontext.Invitations.Where(i => i.UserName == userName).SingleOrDefault();

                if (invite != null && invite.Email == email)
                {
                    invite.Accepted = Extras.DateTimeToday();
                    opiddailycontext.SaveChanges();
                    return invite;
                }

                return null;
            }
        }

        public static void ExtendInvitation(InvitationViewModel ivm)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Invitation invite = InvitationViewModelToInvite(ivm);
                   
                opiddailycontext.Invitations.Add(invite);
                opiddailycontext.SaveChanges();
            }

            //  SendEmailInvitation(invitation);
        }

        private static string GetAgencyById(int agencyId)
        {

            if (agencyId == 0)
            {
                return "OpID";
            }
            else
            {
                using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
                {
                    foreach (Agency agency in opiddailycontext.Agencies)
                    {
                        if (agency.AgencyId == agencyId)
                        {
                            return agency.AgencyName;
                        }
                    }

                    return "Unknown agency";
                }
            }
        }

        private static InvitationViewModel InviteToInvitationViewModel(Invitation invite)
        {
            string agency = GetAgencyById(invite.AgencyId); ;
                       
            return new InvitationViewModel
            {
                Id = invite.Id,
                Extended = invite.Extended.ToString("MM/dd/yyyy"),
                Accepted = (invite.Accepted == (System.DateTime)System.Data.SqlTypes.SqlDateTime.Null ? string.Empty : invite.Accepted.ToString("MM/dd/yyyy")),
                UserName = invite.UserName,
                FullName = invite.FullName,
                Email = invite.Email,
                Role = invite.Role,
                Agency = agency,
                AgencyId = invite.AgencyId.ToString()
            };
        }

        private static Invitation InvitationViewModelToInvite(InvitationViewModel ivm)
        {
            return new Invitation
            {
                Extended = Extras.DateTimeToday(),
                Accepted = (System.DateTime)System.Data.SqlTypes.SqlDateTime.Null,
                UserName = ivm.UserName,
                FullName = ivm.FullName,
                Email = ivm.Email,
                Role = ivm.Role,
                AgencyId = Convert.ToInt32(ivm.AgencyId )    
            };
        }

        public static List<InvitationViewModel> GetUsers()
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                List<InvitationViewModel> invitations = new List<InvitationViewModel>();
                List<Invitation> invites = opiddailycontext.Invitations.ToList();

                foreach (Invitation invite in invites)
                {
                    invitations.Add(InviteToInvitationViewModel(invite));
                }

                return invitations;
            }
        }

    }
}