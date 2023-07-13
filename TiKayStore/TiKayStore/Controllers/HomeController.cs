using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using TiKayStore.Models;
namespace TiKayStore.Controllers
{
    public class HomeController : Controller
    {
        PhoneStoreEntities1 db = new PhoneStoreEntities1();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult getBanner()
        {
            var v = from t in db.tb_ProductCategory
                    where t.Hide == true
                    select t;
            return PartialView(v.ToList());
        }
        public ActionResult getCategories()
        {
            var v = from t in db.tb_ProductCategory
                    where t.Hide == true
                    select t;
            return PartialView(v.ToList());
        }
        public ActionResult getProduct()
        {
            var v = (from t in db.tb_Product
                     where t.Hide == true
                     orderby t.CreateDate descending
                     select t).Take(8);
            return PartialView(v.ToList());
        }
        public ActionResult getNews()
        {
            var v = (from t in db.tb_News
                     where t.Hide == true
                     orderby t.CreateDate descending
                     select t).Take(3);
            return PartialView(v.ToList());
        }
        public ActionResult getAdv()
        {
            var v = (from t in db.tb_Advertisement
                     where t.Hide == true
                     select t).Take(1);
            return PartialView(v.ToList());
        }
    }
}