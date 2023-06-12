using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using TiKayStore.Models;

namespace TiKayStore.Areas.Admin.Controllers
{
    public class ProductImageController : Controller
    {
        PhoneStoreEntities1 db = new PhoneStoreEntities1();
        // GET: Admin/ProductImage
        public ActionResult Index(int id)
        {
            ViewBag.ProductId = id;
            var items = db.tb_ProductImage.Where(x => x.productId == id).ToList();
            return View(items);
        }
        [HttpPost]
        public ActionResult AddImage(int productId, string url)
        {
            db.tb_ProductImage.Add(new tb_ProductImage
            {
                productId = productId,
                image = url,
                isDefault = false
            });
            db.SaveChanges();
            return Json(new { Success = true });
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.tb_ProductImage.Find(id);
            db.tb_ProductImage.Remove(item);
            int productId = item.productId;
            tb_ProductImage items = db.tb_ProductImage.Where(y => y.productId == productId && y.id != id).FirstOrDefault();
            tb_Product product = db.tb_Product.Where(y => y.id == productId).FirstOrDefault();
            if (items != null)
            {
                items.isDefault = true;
                product.Image = items.image;
            }


            db.SaveChanges();

            return Json(new { success = true });
        }
        [HttpPost]
        public ActionResult isDefault(int id)
        {
            var item = db.tb_ProductImage.Find(id);

            if (item != null)
            {
                item.isDefault = !item.isDefault;
                int productId = item.productId;
                List<tb_ProductImage> items = db.tb_ProductImage.Where(y => y.productId == productId && y.id != id).ToList();
                tb_Product product = db.tb_Product.Where(y => y.id == productId).FirstOrDefault();
                foreach (tb_ProductImage i in items)
                {
                    i.isDefault = false;
                }
                product.Image = item.image;

            }
            db.SaveChanges();
            return Json(new { success = true });
        }

    }
}