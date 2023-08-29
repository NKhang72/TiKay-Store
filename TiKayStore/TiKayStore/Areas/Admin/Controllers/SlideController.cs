using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using TiKayStore.Models;
using PagedList;


namespace TiKayStore.Areas.Admin.Controllers
{
    public class SlideController : Controller
    {
        PhoneStoreEntities1 db = new PhoneStoreEntities1();

        // GET: Admin/Slide
        public ActionResult Index(int? page, string typeSort)
        {
            ViewBag.TiltleSort = typeSort == "Tiltle" ? "tiltle_desc" : "Tiltle";
            ViewBag.CreateBySort = typeSort == "CreateBy" ? "createBy_desc" : "CreateBy";
            ViewBag.DateSort = typeSort == "Date" ? "date_desc" : "Date";
            ViewBag.HideSort = String.IsNullOrEmpty(typeSort) ? "hide" : "";

            IEnumerable<tb_Advertisement> items = db.tb_Advertisement;
            switch (typeSort)
            {
                case "tiltle_desc":
                    items = items.OrderByDescending(s => s.Title);
                    break;
                case "Tiltle":
                    items = items.OrderBy(s => s.Title);
                    break;
                case "createBy_desc":
                    items = items.OrderByDescending(s => s.CreateBy);
                    break;
                case "CreateBy":
                    items = items.OrderBy(s => s.Title);
                    break;
                case "hide":
                    items = items.OrderBy(s => s.CreateBy);
                    break;

                case "Date":
                    items = items.OrderBy(s => s.CreateDate);
                    break;
                case "date_desc":
                    items = items.OrderByDescending(s => s.CreateDate);
                    break;

                default:
                    items = items.OrderByDescending(s => s.Hide);
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
        public ActionResult IsActive(int id)
        {
            var item = db.tb_Advertisement.Find(id);
            List<tb_Advertisement> listAds= db.tb_Advertisement.ToList();
            foreach(var ad in listAds)
            {
                if(ad.id == id)
                {
                    ad.Hide = true;
                }
                else
                {
                    ad.Hide = false;
                }
                db.tb_Advertisement.Attach(ad);
                db.Entry(ad).Property(x => x.Hide).IsModified = true;
                db.SaveChanges();
            }
            
            return Json(new { message = "Success", Success = true, isAcive = true });
            
        }
        public ActionResult Add()
        {
            UserLogin user = (UserLogin)Session["USER_SESSION"];
            var count = db.tb_Indentify.Count(x => x.IdUser == user.UserId & x.IdPermission == 19);
            if (count > 0)
            {

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
        public ActionResult Add(tb_Advertisement model)
        {
            if (ModelState.IsValid)
            {
                UserLogin userLogin = (UserLogin)Session["USER_SESSION"];
                model.CreateBy = db.tb_User.Find(userLogin.UserId).id;
                model.CreateDate = DateTime.Now;
                model.Hide = false;
                if (string.IsNullOrEmpty(model.Meta))
                    model.Meta = TiKayStore.Models.Common.Filter.FilterChar(model.Title);
               

                db.tb_Advertisement.Add(model);
                try
                {
                    // Your code...
                    // Could also be before try if you know the exception occurs in SaveChanges

                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }

                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            UserLogin user = (UserLogin)Session["USER_SESSION"];
            var count = db.tb_Indentify.Count(x => x.IdUser == user.UserId & x.IdPermission == 20);
            if (count > 0)
            {
                var i = db.tb_Advertisement.Find(id);
                return View(i);

            }
            else
            {
                return RedirectToAction("Unpermitted", "Error");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(tb_Advertisement model)
        {
            if (ModelState.IsValid)
            {
                model.ModifierDate = DateTime.Now;
                model.Meta = model.Meta = TiKayStore.Models.Common.Filter.FilterChar(model.Title);
               
                db.tb_Advertisement.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            UserLogin user = (UserLogin)Session["USER_SESSION"];
            var count = db.tb_Indentify.Count(x => x.IdUser == user.UserId & x.IdPermission == 22);
            if (count > 0)
            {
                var item = db.tb_Advertisement.Find(id);
                if (item != null)
                {
                    db.tb_Advertisement.Remove(item);
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
            var count = db.tb_Indentify.Count(x => x.IdUser == user.UserId & x.IdPermission == 22);
            if (count > 0)
            {
                List<String> listIdString = ids.Split(',').ToList();
                List<int> listId = listIdString.Select(int.Parse).ToList();
                foreach (int id in listId)
                {
                    var item = db.tb_Advertisement.Find(id);
                    if (item != null)
                    {
                        db.tb_Advertisement.Remove(item);
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