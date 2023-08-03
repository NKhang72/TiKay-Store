using TiKayStore.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using PagedList;


namespace TiKayStore.Controllers
{
    public class NewsController : Controller
    {
        // GET: News
        PhoneStoreEntities1 db = new PhoneStoreEntities1();

        public ActionResult Index(int? page)
        {

            IEnumerable<tb_News> items = db.tb_News.Where(x=>x.Hide==true).OrderByDescending(x => x.id);
            var pageSize = 6;
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

        public ActionResult Detail(int id)
        {
            var item = db.tb_News.Find(id);
            return View(item);
        }

        public ActionResult getNewsbyId(int id)
        {
            var v = from t in db.tb_News
                    where t.id == id
                    select t;
            return View(v.FirstOrDefault());
        }
    }
}