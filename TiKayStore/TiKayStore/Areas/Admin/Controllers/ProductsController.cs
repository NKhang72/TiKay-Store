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
    public class ProductsController : Controller
    {
        PhoneStoreEntities1 db = new PhoneStoreEntities1();
        // GET: Admin/Products
        public ActionResult Index(int? page)
        {
            IEnumerable<tb_Product> items = db.tb_Product.OrderByDescending(x => x.id);
            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);
        }
        public ActionResult IndexbyCategori(int? page, int CategoryID)
        {
            IEnumerable<tb_Product> items = db.tb_Product.Where(x => x.tb_ProductCategory.id == CategoryID).OrderByDescending(x => x.id);

            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);
        }
        public ActionResult Add()
        {
            ViewBag.ProductCategory = new SelectList(db.tb_ProductCategory.ToList(), "id", "Title");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Add(tb_Product model, List<string> Images, List<int> rDefault)
        {
            var v = from t in db.tb_Product
                    where t.Hide == true
                    select t;
            model.id = v.ToList().LastOrDefault().id + 1;
            if (ModelState.IsValid)
            {
                if (Images != null && Images.Count > 0)
                {
                    for (int i = 0; i < Images.Count; i++)
                    {
                        if (i + 1 == rDefault[0])
                        {
                            model.Image = Images[i];
                            model.tb_ProductImage.Add(new tb_ProductImage
                            {
                                productId = model.id,
                                image = Images[i],
                                isDefault = true
                            });
                        }
                        else
                        {
                            model.tb_ProductImage.Add(new tb_ProductImage
                            {
                                productId = model.id,
                                image = Images[i],
                                isDefault = false
                            });
                        }
                    }
                }
                model.CreateDate = DateTime.Now;
                model.ModifierDate = DateTime.Now;
                var hide = Request.Form.GetValues("hide")[0];
                var sale = Request.Form.GetValues("sale")[0];
                if (hide == "true")
                {
                    model.Hide = true;
                }
                else
                {
                    model.Hide = false;
                }
                if (sale == "true")
                {
                    model.Sale = true;
                }
                else
                {
                    model.Sale = false;
                }
                if (string.IsNullOrEmpty(model.SeoTitle))
                {
                    model.SeoTitle = model.Title;
                }
                if (string.IsNullOrEmpty(model.Meta))
                    model.Meta = TiKayStore.Models.Common.Filter.FilterChar(model.Title);
                db.tb_Product.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductCategory = new SelectList(db.tb_ProductCategory.ToList(), "id", "Title");
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            ViewBag.ProductCategory = new SelectList(db.tb_ProductCategory.ToList(), "id", "Title");
            var item = db.tb_Product.Find(id);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(tb_Product model)
        {
            if (ModelState.IsValid)
            {
                model.ModifierDate = DateTime.Now;
                model.Meta = model.Meta = TiKayStore.Models.Common.Filter.FilterChar(model.Title);
                db.tb_Product.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.tb_Product.Find(id);
            if (item != null)
            {
                var checkImg = item.tb_ProductImage.Where(x => x.productId == item.id).ToList();
                if (checkImg != null)
                {
                    foreach (var img in checkImg)
                    {
                        db.tb_ProductImage.Remove(img);
                        db.SaveChanges();
                    }
                }
                db.tb_Product.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult deleteAll(String ids)
        {
            List<String> listIdString = ids.Split(',').ToList();
            List<int> listId = listIdString.Select(int.Parse).ToList();
            foreach (int id in listId)
            {
                var item = db.tb_Product.Find(id);
                if (item != null)
                {
                    var checkImg = item.tb_ProductImage.Where(x => x.productId == item.id).ToList();
                    if (checkImg != null)
                    {
                        foreach (var img in checkImg)
                        {
                            db.tb_ProductImage.Remove(img);
                            db.SaveChanges();
                        }
                    }
                    db.tb_Product.Remove(item);
                    db.SaveChanges();
                }
            }
            return Json(new { success = true });
        }
    }
}