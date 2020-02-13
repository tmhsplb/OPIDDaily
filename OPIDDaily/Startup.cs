using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using OPIDDaily.DataContexts;
using OPIDDaily.Models;
using Owin;

[assembly: OwinStartupAttribute(typeof(OPIDDaily.Startup))]
namespace OPIDDaily
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ConfigureAuth(app);
            CreateRolesAndUsers();
        }

        private void CreateRolesAndUsers()
        {
            IdentityDB context = new IdentityDB();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!roleManager.RoleExists("SuperAdmin"))
            {
                // First create SuperAdmin role
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "SuperAdmin";
                roleManager.Create(role);

                // Here we create the Superadmin user (sa) who will maintain the website				
                var user = new ApplicationUser();
                user.UserName = "sa";
                user.Email = Config.SuperadminEmail;
                string userPwd = Config.SuperadminPassword;
                var chkUser = UserManager.Create(user, userPwd);

                // Add default User to Role Admin. Password configured on Web.config.
                if (chkUser.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "SuperAdmin");
                }
            }
        }
    }
}
