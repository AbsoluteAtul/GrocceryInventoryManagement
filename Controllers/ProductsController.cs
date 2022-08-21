using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using prjGroceryStore4;

namespace prjGroceryStore4.Controllers
{
    public class ProductsController : Controller
    {
        private inventoryDBEntities db = new inventoryDBEntities();

        // GET: Products
        public ActionResult Index(string searching)
        {
            Authenticate("Products/Index");
            var products = db.products.Include(p => p.category);
            // For search option
            return View(db.products.Where(x => x.pro_name.Contains(searching) || searching == null).ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        [HttpGet]
        // GET: Products/Create
        public ActionResult Create()
        {
            Authenticate("Products/Create");
            ViewBag.pro_catID = new SelectList(db.categories, "cat_id", "cat_name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(product model, HttpPostedFileBase ImageFile)
        {

            ViewBag.pro_catID = new SelectList(db.categories, "cat_id", "cat_name", model.pro_catID);
           
            // To store the image path inside the variable and save the selected image in images folder with the date appended with the name of the file
            string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                string extension = Path.GetExtension(ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                model.pro_image = "/image/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/image/"), fileName);
                ImageFile.SaveAs(fileName);

            if (ModelState.IsValid)
            {
                try
                {
                    db.products.Add(model);
                    db.SaveChanges();
                    ModelState.Clear();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // for uniqueness of the products
                    ViewBag.BarcodeError = "Barcode already exists";
                }
            }

            return View(model);
        }


        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            // To extract the data of categories inside product
            ViewBag.pro_catID = new SelectList(db.categories, "cat_id", "cat_name",product.pro_catID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(product product, HttpPostedFileBase ImageFile)
        //public ActionResult Edit([Bind(Include = "pro_id,pro_name,pro_barcode,pro_location,pro_quantity,p_description,p_price,pro_catID,pro_image")]product product, HttpPostedFileBase ImageFile)
        {
            ViewBag.pro_catID = new SelectList(db.categories, "cat_id", "cat_name",product.pro_catID);

            //ModelState.Remove(nameof(product.pro_image));
            if (ModelState.IsValid)
            {
                string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                string extension = Path.GetExtension(ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                product.pro_image = "/image/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/image/"), fileName);
                ImageFile.SaveAs(fileName);

                try
                {
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }catch(Exception ex)
                {
                    ViewBag.BarcodeError = "Barcode already exists";
                }
            }
            return View(product);
        }

        // GET: Products/Delete/5
       public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            product product = db.products.Find(id);
            db.products.Remove(product);
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
