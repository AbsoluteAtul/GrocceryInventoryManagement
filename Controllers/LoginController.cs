using prjGroceryStore4.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjGroceryStore4.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            ViewBag.RetunUrl = Request.QueryString["ReturnUrl"];
            user user = new user();
            return View(user);
        }
        [HttpPost]
        public ActionResult Index(UserProfile user)
        {
            inventoryDBEntities obj = new inventoryDBEntities();

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["inventoryDB2"].ConnectionString);
            try
            {

                con.Open();
                string qry = "select * from users where u_email='" + user.Username + "' or u_username='"+user.Username+"' and u_password='" + user.Password + "'";
                SqlCommand cmd = new SqlCommand(qry, con);
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                   // next time the same person come cookie will identify him
                    HttpCookie cookie = new HttpCookie("AuthCookie");
                    HttpCookie roleCookie = new HttpCookie("Role");
                    HttpCookie IdCookie = new HttpCookie("ID");
                    cookie.Value = user.Username;
                
                    if (user.RememberMe == true)
                    {
                        // if user checked remember me cookie should be persistent
                        cookie.Expires = DateTime.Now.AddDays(7);
                        roleCookie.Expires = DateTime.Now.AddDays(7);
                    }
                    cookie.Path = Request.ApplicationPath;
                    Response.Cookies.Add(cookie);
                    string role = obj.st_getRoleuserNew(user.Username).Single();
                    roleCookie.Value = role;
                    roleCookie.Path = Request.ApplicationPath;
                    Response.Cookies.Add(roleCookie);

                    
                    
                    var id = Convert.ToInt32(obj.st_getUserId(user.Username).Single());
                    
                    IdCookie.Value = id.ToString();
                    IdCookie.Path = Request.ApplicationPath;
                    Response.Cookies.Add(IdCookie);
                    //if the returnurl is there it will show the same page after authentication or it will go to the index
                    string return_url = Request.QueryString["ReturnUrl"];
                    if (string.IsNullOrEmpty(return_url))
                    {
                        return Redirect("Products/Index");
                    }
                    else
                    {
                        return Redirect(return_url);
                    }
                    // redirect to the Index

                }
                else
                {
                    ViewBag.Error = "Invalid UserName or Password";
                }
            }
            catch (Exception e)
            {
                Response.Write(e.Message);
            }

            con.Close();
            return View(user);
        }
        public ActionResult Logout()
        {
            HttpCookie cookie = Request.Cookies["AuthCookie"];
            HttpCookie roleCookie = Request.Cookies["Role"];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
            }
            if(roleCookie != null)
            {
                roleCookie.Expires = DateTime.Now.AddDays(-1);

            }
            
            //roleCookie.Path = Request.ApplicationPath;
            //Response.Cookies.Add(roleCookie);

            cookie.Path = Request.ApplicationPath;
            Response.Cookies.Add(cookie);
            return Redirect("/Login/Index");
        }
    }
}