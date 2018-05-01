using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShopBanHang.Models;

namespace ShopBanHang.Controllers
{
    public class CartsController : Controller
    {
        private WatchShopDbContext db = new WatchShopDbContext();

        // GET: Carts
        public ActionResult Index()
        {
            var carts = db.Carts.Include(c => c.Customer).Include(c => c.Product);
            return View(carts.ToList());
        }


        

        public ActionResult AddCart()
        {
            List<CartItem> carts = Session["giohang"] as List<CartItem>;
            if (carts == null)
            {
                return RedirectToAction("GioHangIndex", "Home");
            }
            if (Session["user"] as string == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string i = Session["user"].ToString();
            var id = db.Customers.FirstOrDefault(x => x.Usernames == i);
            List<Cart> l = new List<Cart>();
            foreach (var item in carts)
            {

                Cart c = new Cart
                {
                    TotalMoney = item.TotalMoney,
                    Status = false,
                    CustomerID = id.CustomerID,
                    DateCreate = DateTime.Now,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity
                };
                l.Add(c);
            }
            db.Carts.AddRange(l);
            db.SaveChanges();
            carts.Clear();
            Session["message"] = "Thành công";
            return RedirectToAction("GioHangIndex", "Home");
        }


        // GET: Carts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // GET: Carts/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Name");
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "Name");
            return View();
        }
        // GET: Carts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Name", cart.CustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "Name", cart.ProductID);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CartID,CustomerID,DateCreate,ProductID,Quantity,Status,TotalMoney")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cart).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Name", cart.CustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "Name", cart.ProductID);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cart cart = db.Carts.Find(id);
            db.Carts.Remove(cart);
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
