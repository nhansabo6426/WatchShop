using ShopBanHang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ShopBanHang.Controllers
{
    public class AdminController : Controller
    {
        private readonly WatchShopDbContext db = new WatchShopDbContext(); 
        // GET: Admin
        public ActionResult LoginAdmin()
        {
            return View();
        }

        //Mã hóa Password
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
        public ActionResult LoginAdmin(FormCollection f)
        {
            var username = f["username"].ToString();
            var password = f["password"].ToString();
            var has = CreateMD5(password);
            var key = "admin";
            var ad = db.Customers.
                FirstOrDefault(x => x.Usernames == username && x.HashPassword == has && x.AccessManagement == key);
            if(ad == null)
            {
                ViewBag.message = "Login fail";
                return View();
            }
            ViewBag.message = "Login success";
            return RedirectToAction("HomeAdmin");
        }


        public ActionResult HomeAdmin()
        {
            return View();
        }
    }
}