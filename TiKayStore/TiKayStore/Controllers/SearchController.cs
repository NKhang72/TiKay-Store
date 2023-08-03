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
    public class SearchController : Controller
    {
        PhoneStoreEntities1 db = new PhoneStoreEntities1();
        // GET: Search
        public ActionResult Search(string searchString)
        {
            if(searchString == "")
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var links = from l in db.tb_Product
                            where l.Title.Contains(searchString)// lấy toàn bộ liên kết
                            select l;


                return View(links.ToList());
            }
            

           
        }
        public ActionResult FilterbyPrice(int sPrice, int ePrice, int categoryId, int? page)
        {
            categoryId = (int)Session["category"];
           
            IEnumerable<tb_Product> items = db.tb_Product.Where(t => t.Price >= sPrice && t.Price <= ePrice && t.tb_ProductCategory.id == categoryId).OrderByDescending(x => x.id);
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
    }
}