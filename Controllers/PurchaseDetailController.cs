using prjGroceryStore4.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace prjGroceryStore4.Controllers
{
    public class PurchaseDetailController : Controller
    {
        inventoryDBEntities db = new inventoryDBEntities();
        // GET: PurchaseDetail
        public ActionResult Index(string searching)
        {
            Authenticate("Products/Index");

            // return View(db.purchaseDetails.Where(x => x.product.pro_name.Contains(searching) || searching == null).ToList());

            // TO display purchases in the order of their latest dates
            return View(db.purchaseDetails.OrderByDescending(x => x.purchase.p_date).ToList());
        }
        public ActionResult Create()
        {
            Authenticate("PurchaseDetail/Create");

            // Extracting information from supplier and products
            List<supplier> list = db.suppliers.ToList();
            ViewBag.suppList = new SelectList(list, "sup_id", "sup_name");

            List<product> list2 = db.products.ToList();
            ViewBag.proList = new SelectList(list2, "pro_id", "pro_name");
            return View();
        }

        [HttpPost]
        public ActionResult Create(PurchaseViewModel model)
        {
            try
            {
                // storing the information into three tables 

                inventoryDBEntities db = new inventoryDBEntities();
                List<supplier> list = db.suppliers.ToList();
                ViewBag.suppList = new SelectList(list, "sup_id", "sup_name");

                List<product> list2 = db.products.ToList();
                ViewBag.proList = new SelectList(list2, "pro_id", "pro_name");
                purchase finPurchase = db.purchases.Find(model.PurchaseId);
                if (finPurchase == null)
                {
                    purchase pur = new purchase();
                    pur.p_date = model.PurchaseDate;
                    pur.p_suppId = model.SupplierID;

                    db.purchases.Add(pur);
                    db.SaveChanges();

                    long latestpurId = pur.p_id;

                    purchaseDetail detail = new purchaseDetail();
                    detail.pd_purchaseId = latestpurId;
                    detail.pd_proId = model.ProductID;
                    detail.pd_quantity = model.ProductQuantity;
                    detail.pd_buyingPrice = model.ProductBuyingPrice;

                    db.purchaseDetails.Add(detail);
                    db.SaveChanges();

                    product pro = db.products.Find(model.ProductID);

                    if (pro == null)
                    {
                        return HttpNotFound();
                    }
                    else
                    {
                        if (ModelState.IsValid)
                        {
                            pro.pro_quantity = pro.pro_quantity + model.ProductQuantity;
                            db.Entry(pro).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            ViewBag.Error = "Wrong";
                        }
                    }
               
                }
                else
                {

                    long latestpurId = finPurchase.p_id;

                    purchaseDetail detail = new purchaseDetail();
                    detail.pd_purchaseId = latestpurId;
                    detail.pd_proId = model.ProductID;
                    detail.pd_quantity = model.ProductQuantity;
                    detail.pd_buyingPrice = model.ProductBuyingPrice;

                    db.purchaseDetails.Add(detail);
                    db.SaveChanges();

                    product pro = db.products.Find(model.ProductID);

                    if (pro == null)
                    {
                        return HttpNotFound();
                    }
                    else
                    {

                            pro.pro_quantity = model.ProductQuantity + pro.pro_quantity;
                            db.Entry(pro).State = EntityState.Modified;
                            db.SaveChanges();
                     
                    }
                    
                }
            }
            catch(Exception ex)
            {
                Response.Write(ex.Message);
            }

            return View(model);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            purchaseDetail purchasedetail = db.purchaseDetails.Find(id);
            if (purchasedetail == null)
            {
                return HttpNotFound();
            }
            return View(purchasedetail);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            purchaseDetail purchasedetail = db.purchaseDetails.Find(id);
            db.purchaseDetails.Remove(purchasedetail);
            db.SaveChanges();
            return RedirectToAction("Index");
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