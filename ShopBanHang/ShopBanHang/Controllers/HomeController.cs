using ShopBanHang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopBanHang.Controllers
{
    public class HomeController : Controller
    {
        WatchShopDbContext db = new WatchShopDbContext();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult ListProduct()
        {
            var list = db.Products.ToList();
            return View(list);
        }

        public ActionResult DetailProduct(int? id)
        {
            if(id == null)
            {
                ViewBag.message = "Không tìm thấy";
                return View();
            }
            var list = db.Products.FirstOrDefault(x => x.ProductID == id);
            return View(list);
        }

        public ActionResult Contact()
        {
            return View();
        }


        public ActionResult LoaiSanPham(int? producttype)
        {
            if (producttype == null)
            {
                return RedirectToAction("ListProduct","Home");
            }
            
            
                var result = db.Products.Where(x => x.ProductTypeID == producttype).ToList();
                
            return View(result);
        }

        // Trang giỏ hàng
        public ActionResult GioHangIndex()
        {
            var listCart = Session["giohang"] as List<CartItem>;
            if (listCart == null)
            {
                ViewBag.number = 0;
                ViewBag.money = 0;
            }
            else
            {
                ViewBag.number = listCart.Sum(x => x.Quantity);
                ViewBag.money = listCart.Sum(x => x.TotalMoney * x.Quantity);
            }

            return View(listCart);
        }


        //Đổ sản phẩm từ giỏ hàng vào CSDL 
        public RedirectToRouteResult AddCart()
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
            return RedirectToAction("giohang", "Home");
        }

        public RedirectToRouteResult ThemVaoGio(int SanPhamID)
        {
            if (Session["giohang"] == null) // Nếu giỏ hàng chưa được khởi tạo
            {
                Session["giohang"] = new List<CartItem>();  // Khởi tạo Session["giohang"] là 1 List<CartItem>
            }

            List<CartItem> giohang = Session["giohang"] as List<CartItem>;  // Gán qua biến giohang dễ code

            // Kiểm tra xem sản phẩm khách đang chọn đã có trong giỏ hàng chưa

            if (giohang.FirstOrDefault(m => m.ProductID == SanPhamID) == null) // ko co sp nay trong gio hang
            {
                Product sp = db.Products.Find(SanPhamID);  // tim sp theo sanPhamID

                CartItem newItem = new CartItem()
                {
                    ProductID = SanPhamID,
                    TenSanPham = sp.Name,
                    Quantity = 1,
                    Hinh = sp.Images1,
                    DonGia = (decimal)sp.Price,
                    TotalMoney = sp.Price
                };  // Tạo ra 1 CartItem mới

                giohang.Add(newItem);  // Thêm CartItem vào giỏ 
            }
            else
            {
                // Nếu sản phẩm khách chọn đã có trong giỏ hàng thì không thêm vào giỏ nữa mà tăng số lượng lên.
                CartItem cardItem = giohang.FirstOrDefault(m => m.ProductID == SanPhamID);
                cardItem.Quantity++;
            }

            // Action này sẽ chuyển hướng về trang chi tiết sp khi khách hàng đặt vào giỏ thành công. Bạn có thể chuyển về chính trang khách hàng vừa đứng bằng lệnh return Redirect(Request.UrlReferrer.ToString()); nếu muốn.
            return RedirectToAction("ListProduct", "Home", new { id = SanPhamID });
        }



        //Sửa số lượng sản phẩm
        public RedirectToRouteResult SuaSoLuong(int SanPhamID, int soluongmoi)
        {
            // tìm carditem muon sua
            List<CartItem> giohang = Session["giohang"] as List<CartItem>;
            CartItem itemSua = giohang.FirstOrDefault(m => m.ProductID == SanPhamID);
            if (itemSua != null)
            {
                itemSua.Quantity = soluongmoi;
            }
            return RedirectToAction("GioHangIndex");

        }


        //Xóa sản phẩm khỏi giỏ hàng
        public RedirectToRouteResult XoaKhoiGio(int SanPhamID)
        {
            List<CartItem> giohang = Session["giohang"] as List<CartItem>;
            CartItem itemXoa = giohang.FirstOrDefault(m => m.ProductID == SanPhamID);
            if (itemXoa != null)
            {
                giohang.Remove(itemXoa);
            }
            return RedirectToAction("GioHangIndex");

        }

    }
}