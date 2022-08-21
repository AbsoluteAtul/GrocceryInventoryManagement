using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using prjGroceryStore4;

namespace prjGroceryStore4.Controllers
{
    public class UsersController : Controller
    {
        private inventoryDBEntities db = new inventoryDBEntities();

        // GET: Users
        public ActionResult Index()
        {
            Authenticate("Users/Index");
            var users = db.users.Include(u => u.role);
            return View(users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            Authenticate("Users/Details");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            
            MakeList();
            ViewBag.u_roleId = new SelectList(db.roles, "r_id", "r_name");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "u_id,u_name,u_username,u_password,u_phone,u_email,u_status,u_roleId,u_st_address,u_city")] user user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.users.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }catch(Exception ex)
                {
                    ViewBag.EmailError = "User already exists";
                }
            }

            ViewBag.u_roleId = new SelectList(db.roles, "r_id", "r_name", user.u_roleId);
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            Authenticate("Users/Edit");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MakeList();
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.u_roleId = new SelectList(db.roles, "r_id", "r_name", user.u_roleId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "u_id,u_name,u_username,u_password,u_phone,u_email,u_status,u_roleId,u_st_address,u_city")] user user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.EmailError = "User already exists";
                }
            }
            ViewBag.u_roleId = new SelectList(db.roles, "r_id", "r_name", user.u_roleId);
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            Authenticate("Users/Delete");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            user user = db.users.Find(id);
            db.users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private void MakeList()
        {
            // For Passing the values inside role status
            List<SelectListItem> li = new List<SelectListItem>();// will contain all the dropdown values
            li.Add(new SelectListItem() { Text = "Active", Value = "1" });
            li.Add(new SelectListItem() { Text = "InActive", Value = "0" });
            ViewBag.abc = new SelectList(li, "Value", "Text");
        }
        private void Authenticate(string returnUrl)
        {

            HttpCookie cookie = Request.Cookies["AuthCookie"];
            HttpCookie roleCookie = Request.Cookies["Role"];
            //if the user didnt logged in the cookie will be null 
            if (cookie == null || roleCookie == null)
            {

                Response.Redirect("/Login/Index?ReturnUrl=" + returnUrl, false);
            }
            if(roleCookie != null && roleCookie.Value != "admin")
            {
                Response.Redirect("/Products");
            }
        }
    }
}
