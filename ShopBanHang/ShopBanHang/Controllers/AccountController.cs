using ShopBanHang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ShopBanHang.Controllers
{
    public class AccountController : Controller
    {
        private readonly WatchShopDbContext db = new WatchShopDbContext();
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection f)
        {
            var pass = CreateMD5(f["password"].ToString());
            var user = f["email"].ToString();
            var result = db.Customers.FirstOrDefault(x => x.Usernames == user && x.HashPassword == pass);
            if(result == null)
            {
                ViewBag.message = "fail";
                return RedirectToAction("Login");
            }
            Session["user"] = user;
            ViewBag.message = "success";
            return RedirectToAction("ListProduct", "Home");
        }

        public string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

        [HttpPost]
        public ActionResult SignUp(FormCollection f)
        {
            string username = f["email"].ToString();
            var password = f["password"].ToString();

            var us = db.Customers.FirstOrDefault(x => x.Usernames == username);
            if(us != null)
            {
                ViewBag.message = "Tài khoản đã tồn tại !";
                return RedirectToAction("SignUp");
            }
            var pass = CreateMD5(password);
            Customer c = new Customer
            {
                Usernames = f["email"].ToString(),
                Address = f["address"].ToString(),
                Password = f["password"].ToString(),
                Name = f["name"].ToString(),
                PhoneNumbers = f["phone"].ToString(),
                HashPassword = pass
            };
            db.Customers.Add(c);
            var result = db.SaveChanges();
            if(result > 0)
            {
                return RedirectToAction("ListProduct", "Home");
            }
            else
            {
                ViewBag.message = "Fail";
                return RedirectToAction("SignUp");
            }
        }
    }
}