using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using TiKayStore.Models;
using PagedList;
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
                    new SelectListItem { Selected = false, Text = "Chuyển khoảng", Value = "2"},
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
                Random rd = new Random();
                model.Code = model.TypePay + model.Phone + rd.Next(0, 100);
                db.tb_Order.Add(model);
                db.SaveChanges();
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