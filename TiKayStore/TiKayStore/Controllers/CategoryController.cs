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
    public class CategoryController : Controller
    {
        PhoneStoreEntities1 db = new PhoneStoreEntities1();
        // GET: Category
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult getProductbyMeta(int id, int? page)
        {
            
            IEnumerable<tb_Product> items = db.tb_Product.Where(x=> x.ProductCategory==id && x.Hide==true).OrderByDescending(x=>x.id);
            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            Session["category"] = items.ToList()[0].tb_ProductCategory.id;
            return View(items);
            
           
        }
        public ActionResult getCategoriesSlideBar()
        {
            var v = from t in db.tb_ProductCategory
                    where t.Hide == true
                    select t;
            return PartialView(v.ToList());
        }
    }
}