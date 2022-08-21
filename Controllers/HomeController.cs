using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjGroceryStore4.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        private void Authenticate(string returnUrl)
        {

            HttpCookie cookie = Request.Cookies["AuthCookie"];
            //if the user didnt logged in the cookie will be null 
            if (cookie == null)
            {
                Response.Redirect("/Login/Index?ReturnUrl=" + returnUrl, false);
            }
        }
    }
}