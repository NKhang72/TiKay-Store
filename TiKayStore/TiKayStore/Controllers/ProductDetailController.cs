using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using TiKayStore.Models;

namespace TiKayStore.Controllers
{
    public class ProductDetailController : Controller
    {
        PhoneStoreEntities1 db = new PhoneStoreEntities1();
        // GET: ProductDetail

        public ActionResult getProductDetails(int id)
        {
            var v = from t in db.tb_Product
                    where t.id == id
                    select t;
            return View(v.FirstOrDefault());
        }
       
    }
}