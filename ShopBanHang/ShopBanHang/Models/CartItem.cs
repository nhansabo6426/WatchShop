using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopBanHang.Models
{
    public class CartItem : Cart
    {
        public string Hinh { get; set; }
        public string TenSanPham { get; set; }
        public decimal DonGia { get; set; }
    }
}