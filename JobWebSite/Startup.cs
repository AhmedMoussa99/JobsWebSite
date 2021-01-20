using JobWebSite.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JobWebSite.Startup))]
namespace JobWebSite
{
    public partial class Startup
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreatDefaultRolesAndUsers();
        }
        public void CreatDefaultRolesAndUsers()
        {
            var rolemanger = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var usermanger = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            IdentityRole role = new IdentityRole();
            if (!rolemanger.RoleExists("Admins"))
            {
                role.Name = "Admins";
                rolemanger.Create(role);
                ApplicationUser user = new ApplicationUser();
                user.UserName = "Ahmed";
                user.Email = "Ahmed@gmail.com";
                var check = usermanger.Create(user , "@Hmed123");
                if (check.Succeeded)
                {
                    usermanger.AddToRole(user.Id, "Admins");
                }
            }
        }
    }
}
