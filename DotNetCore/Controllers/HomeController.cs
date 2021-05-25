using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DotNetCore.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;

namespace DotNetCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration config;
        private readonly IHttpContextAccessor accessor;

        public HomeController(IHttpContextAccessor contextaccessor, IConfiguration configuration)
        {
            config = configuration;
            accessor = contextaccessor;
        }

        public IActionResult Index()
        {
            
            //read value from config.
            string ValueFromConfig = config.GetSection("AppSettings").GetSection("MyEnvironmentName").Value;

            //use sessions.
            accessor.HttpContext.Session.SetString("MyName", "Manas");
            string Name = accessor.HttpContext.Session.GetString("MyName");
            
            // Get current user id of the accessor (user who is running the browser)
            ClaimsPrincipal User = accessor.HttpContext.User;
            string DomainUserID = User.Identity.Name;

            //Query active directory using the user id to get Current user class
            var DirectoryContext = new PrincipalContext(ContextType.Domain);
            UserPrincipal CurrentUser = UserPrincipal.FindByIdentity(DirectoryContext, DomainUserID);

            List<string> Groups = new List<string>();

            //Get group names where this user belongs to.
            WindowsIdentity winid = (WindowsIdentity)User.Identity;
            foreach (var group in winid.Groups)
            {
                Groups.Add(group.Translate(typeof(NTAccount)).ToString());
            }


            ViewBag.DomainUserID = DomainUserID;

            return View();
        }

    }
}
