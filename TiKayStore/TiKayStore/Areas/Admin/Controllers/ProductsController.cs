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
        
        public ActionResult Sort(int? page, string Category, string currentCategory, string typeSort, string currentFilter, string searchString)
        {
            ViewBag.CurrentSort = typeSort;

            ViewBag.PriceSort = typeSort == "Price" ? "price_desc" : "Price";
            ViewBag.Originalprice=typeSort == "Originalprice" ? "originalprice_desc" : "Originalprice";
            ViewBag.PriceSaleSort = typeSort == "priceSale" ? "priceSale_desc" : "priceSale";
            ViewBag.DateSort = typeSort == "Date" ? "date_desc" : "Date";
            ViewBag.HideSort = typeSort == "Hide" ? "hide_desc" : "Hide";
            ViewBag.SaleSort = typeSort == "Sale" ? "sale_desc" : "Sale";
            ViewBag.QuanitySort = typeSort == "Quanity" ? "quanity_desc" : "Quanity";
            ViewBag.CategorySort = typeSort == "Category" ? "category_desc" : "Category";
            ViewBag.TittleSort = String.IsNullOrEmpty(typeSort) ? "tiltle_desc" : "";

            if (searchString != null || Category!=null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
                Category = currentCategory;
            }

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentCategory = Category;
            IEnumerable <tb_Product> items = db.tb_Product;

            if (!String.IsNullOrEmpty(searchString))
            {
                items = db.tb_Product.Where(x => x.Title.Contains(searchString));
            }
            if (!String.IsNullOrEmpty(Category))
            {
                items = db.tb_Product.Where(x => x.tb_ProductCategory.Title==Category);
            }

            switch (typeSort)
            {
                case "tiltle_desc":
                    items = items.OrderByDescending(s => s.Title);
                    break;

                case "Price":
                    items = items.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    items = items.OrderByDescending(s => s.Price);
                    break;
                case "Originalprice":
                    items = items.OrderBy(s => s.Originalprice);
                    break;
                case "originalprice_desc":
                    items = items.OrderByDescending(s => s.Originalprice);
                    break;
                case "priceSale":
                    items = items.OrderBy(s => s.PriceSale);
                    break;
                case "priceSale_desc":
                    items = items.OrderByDescending(s => s.PriceSale);
                    break;

                case "Hide":
                    items = items.OrderBy(s => s.Hide);
                    break;
                case "hide_desc":
                    items = items.OrderByDescending(s => s.Hide);
                    break;

                case "Sale":
                    items = items.OrderBy(s => s.Sale);
                    break;
                case "sale_desc":
                    items = items.OrderByDescending(s => s.Sale);
                    break;

                case "Quanity":
                    items = items.OrderBy(s => s.Quantity);
                    break;
                case "quanity_desc":
                    items = items.OrderByDescending(s => s.Quantity);
                    break;


                case "Category":
                    items = items.OrderBy(s => s.tb_ProductCategory.Title);
                    break;
                case "category_desc":
                    items = items.OrderByDescending(s => s.tb_ProductCategory.Title);
                    break;

                case "Date":
                    items = items.OrderBy(s => s.CreateDate);
                    break;
                case "date_desc":
                    items = items.OrderByDescending(s => s.CreateDate);
                    break;

                default:
                    items = items.OrderBy(s => s.Title);
                    break;
            }
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
            UserLogin user = (UserLogin)Session["USER_SESSION"];
            var count = db.tb_Indentify.Count(x => x.IdUser == user.UserId & x.IdPermission == 5);
            if (count > 0)
            {
                ViewBag.ProductCategory = new SelectList(db.tb_ProductCategory.ToList(), "id", "Title");
                return View();
            }
            else
            {
                return RedirectToAction("Unpermitted", "Error");
            }
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
                return RedirectToAction("Sort");
            }
            ViewBag.ProductCategory = new SelectList(db.tb_ProductCategory.ToList(), "id", "Title");
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            UserLogin user = (UserLogin)Session["USER_SESSION"];
            var count = db.tb_Indentify.Count(x => x.IdUser == user.UserId & x.IdPermission == 8);
            if (count > 0)
            {
                ViewBag.ProductCategory = new SelectList(db.tb_ProductCategory.ToList(), "id", "Title");
                var item = db.tb_Product.Find(id);
                return View(item);
            }
            else
            {
                return RedirectToAction("Unpermitted", "Error");
            }
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
                return RedirectToAction("Sort");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            UserLogin user = (UserLogin)Session["USER_SESSION"];
            var count = db.tb_Indentify.Count(x => x.IdUser == user.UserId & x.IdPermission == 6);
            if (count > 0)
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
            else
            {
                return Json(new { success = false });

            }
        }
        [HttpPost]
        public ActionResult deleteAll(String ids)
        {
            UserLogin user = (UserLogin)Session["USER_SESSION"];
            var count = db.tb_Indentify.Count(x => x.IdUser == user.UserId & x.IdPermission == 7);
            if (count > 0)
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
            else
            {
                return Json(new { success = false });

            }
        }
    }
}