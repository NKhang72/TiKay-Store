using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using TiKayStore.Models;

namespace TiKayStore.Areas.Admin.Controllers
{
    public class MenuController : Controller
    {
        PhoneStoreEntities1 db = new PhoneStoreEntities1();
        // GET: Admin/Menu
        public ActionResult Index()
        {

            var v = from t in db.tb_Menu
                    where t.Hide == true
                    select t;
            return View(v.ToList());
        }
        public ActionResult ShowHide()
        {
            var v = from t in db.tb_Menu
                    where t.Hide == false
                    select t;
            return View(v.ToList());
        }
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(tb_Menu model)
        {
            if (ModelState.IsValid)
            {
                model.CreateDate = DateTime.Now;
                model.ModifierDate = DateTime.Now;
                model.Meta = TiKayStore.Models.Common.Filter.FilterChar(model.Title);
                var checkbox = Request.Form.GetValues("CheckBoxId")[0];
                if (checkbox == "true")
                {
                    model.Hide = true;
                }
                else
                {
                    model.Hide = false;
                }
                db.tb_Menu.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public ActionResult Edit(int id)
        {
            var item = db.tb_Menu.Find(id);
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tb_Menu model)
        {
            if (ModelState.IsValid)
            {
                db.tb_Menu.Attach(model);
                model.ModifierDate = DateTime.Now;
                model.Meta = TiKayStore.Models.Common.Filter.FilterChar(model.Title);

                db.Entry(model).Property(x => x.Title).IsModified = true;
                db.Entry(model).Property(x => x.Description).IsModified = true;
                db.Entry(model).Property(x => x.Meta).IsModified = true;
                db.Entry(model).Property(x => x.Hide).IsModified = true;


                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public ActionResult Delete(int id)
        {
            var item = db.tb_Menu.Find(id);
            if (item != null)
            {
                db.tb_Menu.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}