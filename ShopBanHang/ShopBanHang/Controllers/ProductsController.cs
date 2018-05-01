using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShopBanHang.Models;

namespace ShopBanHang.Controllers
{
    public class ProductsController : Controller
    {
        private WatchShopDbContext db = new WatchShopDbContext();

        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.ProductType).Include(p => p.Trade);
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.ProductTypeID = new SelectList(db.ProductTypes, "ProductTypeID", "Name");
            ViewBag.TradeID = new SelectList(db.Trades, "TradeID", "Name");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        public ActionResult Create(Product product , HttpPostedFileBase img1, HttpPostedFileBase img2, HttpPostedFileBase img3, HttpPostedFileBase img4)
        {
            //Thêm hình ảnh vào CSDL
            if (ModelState.IsValid)
            {
                string path1, path2, path3, path4;
                if(img1 == null)
                {
                    path1 = null;
                }
                else
                {
                    path1 = Server.MapPath("~/Imgs/" + Path.GetFileName(img1.FileName));
                    img1.SaveAs(path1);
                    product.Images1 = @"/Imgs/" + img1.FileName;
                }
                if(img2 == null)
                {
                    path2 = null;
                }
                else
                {
                    path2 = Server.MapPath("~/Imgs/" + Path.GetFileName(img2.FileName));
                    img2.SaveAs(path2);
                    product.Images2 = @"/Imgs/" + img2.FileName;
                }
                if(img3 == null)
                {
                    path3 = null;
                }
                else
                {
                    path3 = Server.MapPath("~/Imgs/" + Path.GetFileName(img3.FileName));
                    img3.SaveAs(path3);
                    product.Images3 = @"/Imgs/" + img3.FileName;
                }
                if(img4 == null)
                {
                    path4 = null;
                }
                else
                {
                    path4 = Server.MapPath("~/Imgs/" + Path.GetFileName(img4.FileName));
                    img4.SaveAs(path4);
                    product.Images4 = @"/Imgs/" + img4.FileName;
                }

                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //Đưa dữ liệu vào dropdownlist
            ViewBag.ProductTypeID = new SelectList(db.ProductTypes, "ProductTypeID", "Name", product.ProductTypeID);
            ViewBag.TradeID = new SelectList(db.Trades, "TradeID", "Name", product.TradeID);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductTypeID = new SelectList(db.ProductTypes, "ProductTypeID", "Name", product.ProductTypeID);
            ViewBag.TradeID = new SelectList(db.Trades, "TradeID", "Name", product.TradeID);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,Name,Origin,Price,Description,Quantity,ProductTypeID,TradeID,Images1,Images2,Images3,Images4")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductTypeID = new SelectList(db.ProductTypes, "ProductTypeID", "Name", product.ProductTypeID);
            ViewBag.TradeID = new SelectList(db.Trades, "TradeID", "Name", product.TradeID);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
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
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
    }
}
