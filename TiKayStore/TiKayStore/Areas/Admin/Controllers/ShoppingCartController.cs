using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using TiKayStore.Models;
using PagedList;

namespace TiKayStore.Areas.Admin.Controllers
{
    public class ShoppingCartController : Controller
    {
        PhoneStoreEntities1 db = new PhoneStoreEntities1();
        // GET: Admin/ShoppingCart
        public ActionResult Index(int? page)
        {

            var items = db.tb_Order.OrderByDescending(x => x.CreateDate).ToList();

            if (page == null)
            {
                page = 1;
            }
            var pageNumber = page ?? 1;
            var pageSize = 10;
            ViewBag.PageSize = pageSize;
            ViewBag.Page = pageNumber;
            return View(items.ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public ActionResult Update(int id)
        {
            var item = db.tb_Order.Find(id);
            String payment = item.TypePay;
            if (item != null)
            {
                db.tb_Order.Attach(item);
                if (payment == "1")
                {
                    item.TypePay = "2";
                }
                else
                {
                    item.TypePay = "1";

                }

                db.Entry(item).Property(x => x.TypePay).IsModified = true;
                db.SaveChanges();
                return Json(new { message = "Success", Success = true });
            }
            return Json(new { message = "Unsuccess", Success = false });
        }
        public ActionResult View(int id)
        {
            var item = db.tb_Order.Find(id);
            return View(item);
        }
    }
}