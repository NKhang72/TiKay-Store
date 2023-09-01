using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using TiKayStore.Models;
using PagedList;
using System.Configuration;

namespace TiKayStore.Controllers
{

    public class ShoppingCartController : Controller
    {
        PhoneStoreEntities1 db = new PhoneStoreEntities1();
        // GET: ShoppingCart
        public ActionResult Index()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.CheckCart = cart;
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddToCart(int id, int quantity)
        {
            var code = new { Success = false, msg = "", code = -1, Count = 0 };
            var checkProduct = db.tb_Product.FirstOrDefault(x => x.id == id);
            if (checkProduct != null)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart == null)
                {
                    cart = new ShoppingCart();
                }
                ShoppingCartItem item = new ShoppingCartItem
                {
                    ProductId = checkProduct.id,
                    ProductName = checkProduct.Title,
                    CategoryName = checkProduct.tb_ProductCategory.Title,
                    Meta = checkProduct.Meta,
                    Quantity = quantity
                };
                if (checkProduct.tb_ProductImage.FirstOrDefault(x => x.isDefault.Value) != null)
                {
                    item.ProductImg = checkProduct.tb_ProductImage.FirstOrDefault(x => x.isDefault.Value).image;
                }
                if (checkProduct.Sale == true)
                {
                    item.Price = (decimal)checkProduct.PriceSale;
                }
                else
                {
                    item.Price = (decimal)checkProduct.Price;
                }
                item.TotalPrice = item.Quantity * item.Price;
                cart.AddToCart(item, quantity);
                Session["Cart"] = cart;
                code = new { Success = true, msg = "Thêm sản phẩm vào giở hàng thành công!", code = 1, Count = cart.Items.Count };
            }
            return Json(code);
        }
        public ActionResult ShowCount()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                return Json(new { Count = cart.Items.Count }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Partial_Item_Cart()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var code = new { Success = false, msg = "", code = -1, Count = 0 };

            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                var checkProduct = cart.Items.FirstOrDefault(x => x.ProductId == id);
                if (checkProduct != null)
                {
                    cart.Remove(id);
                    code = new { Success = true, msg = "", code = 1, Count = cart.Items.Count };
                }
            }
            return Json(code);
        }



        [HttpPost]
        public ActionResult DeleteAll()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                cart.ClearCart();
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }
        [HttpPost]
        public ActionResult Update(int id, int quantity)
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                cart.UpdateQuantity(id, quantity);
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }
        public ActionResult CheckOut()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.CheckCart = cart;
            }
            ViewBag.Payment = new SelectList(new List<SelectListItem>
                {

                    new SelectListItem { Selected = true, Text = "COD", Value = "1"},
                    new SelectListItem { Selected = false, Text = "Chuyển khoản", Value = "2"},
                }, "Value", "Text", 1);

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CheckOut(tb_Order model)
        {

            if (ModelState.IsValid)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart != null)
                {
                    cart.Items.ForEach(x => model.tb_OrderDetail.Add(new tb_OrderDetail
                    {
                        ProductId = x.ProductId,
                        Quantity = x.Quantity,
                        Price = x.Price
                    }));
                    model.TotalAmount = cart.Items.Sum(x => (x.Price * x.Quantity));
                }
                model.CreateDate = DateTime.Now;
                model.ModifierDate = DateTime.Now;
                model.CreateBy = model.Phone;
                model.isRead = false;
                Random rd = new Random();
                model.Code = model.TypePay + model.Phone + rd.Next(0, 100);
                db.tb_Order.Add(model);
                db.SaveChanges();

                //send mail cho khachs hang
                var strSanPham = "";
                var thanhtien = decimal.Zero;
                var TongTien = decimal.Zero;
                foreach (var sp in cart.Items)
                {
                    strSanPham += "<tr>";
                    strSanPham += "<td>" + sp.ProductName + "</td>";
                    strSanPham += "<td>" + sp.Quantity + "</td>";
                    strSanPham += "<td>" + TiKayStore.Models.Common.Common.FormatNumber(sp.TotalPrice, 0) + "</td>";
                    strSanPham += "</tr>";
                    thanhtien += sp.Price * sp.Quantity;
                }
                TongTien = thanhtien;
                string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/mail/send2.html"));
                contentCustomer = contentCustomer.Replace("{{MaDon}}", model.Code);
                contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
                contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", model.CustomerName);
                contentCustomer = contentCustomer.Replace("{{Phone}}", model.Phone);
                contentCustomer = contentCustomer.Replace("{{Email}}", model.Email);
                contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", model.Address);
                contentCustomer = contentCustomer.Replace("{{ThanhTien}}", TiKayStore.Models.Common.Common.FormatNumber(thanhtien, 0));
                contentCustomer = contentCustomer.Replace("{{TongTien}}", TiKayStore.Models.Common.Common.FormatNumber(TongTien, 0));
                TiKayStore.Commom.Common.SendMail("TiKayStore", "Đơn hàng #" + model.Code, contentCustomer.ToString(), model.Email);

                string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/mail/send1.html"));
                contentAdmin = contentAdmin.Replace("{{MaDon}}", model.Code);
                contentAdmin = contentAdmin.Replace("{{SanPham}}", strSanPham);
                contentAdmin = contentAdmin.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", model.CustomerName);
                contentAdmin = contentAdmin.Replace("{{Phone}}", model.Phone);
                contentAdmin = contentAdmin.Replace("{{Email}}", model.Email);
                contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", model.Address);
                contentAdmin = contentAdmin.Replace("{{ThanhTien}}", TiKayStore.Models.Common.Common.FormatNumber(thanhtien, 0));
                contentAdmin = contentAdmin.Replace("{{TongTien}}", TiKayStore.Models.Common.Common.FormatNumber(TongTien, 0));
                TiKayStore.Commom.Common.SendMail("TiKayStore", "Đơn hàng mới #" + model.Code, contentAdmin.ToString(), ConfigurationManager.AppSettings["EmailAdmin"]);


                cart.ClearCart();
                return RedirectToAction("Suscess");
            }
            ViewBag.Payment = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Selected = true, Text = "COD", Value = "1"},
                    new SelectListItem { Selected = false, Text = "Chuyển khoảng", Value = "2"},
                }, "Value", "Text", 1);
            return View(model);
        }
        public ActionResult Item_ThanhToan()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }
        public ActionResult Suscess()
        {

            return View();
        }
        public ActionResult SearchOder()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult GetOrderbyPhone(string sdt)
        {

            var items = db.tb_Order.Where(x => x.Phone == sdt).OrderByDescending(x => x.CreateDate);
            return View(items.ToList());
        }
        public ActionResult ViewOrder(int id)
        {
            var item = db.tb_Order.Find(id);
            return View(item);
        }

    }

}