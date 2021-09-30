using System.Web.Mvc;

namespace CinemaBooking.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            /* Dashboard*/
            context.MapRoute(
                "Dashboard",
                "Admin",
                new { Controller = "Admin", action = "Dashboard", id = UrlParameter.Optional }
            );
            /* Login*/
            context.MapRoute(
                "AuthLogin",
                "Admin/Login",
                new { Controller = "Auth", action = "Login", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}