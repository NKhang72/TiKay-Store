﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using TiKayStore.Models;

namespace TiKayStore.Areas.Admin.Controllers
{
    public class NewsController : Controller
    {
        PhoneStoreEntities1 db = new PhoneStoreEntities1();
        // GET: Admin/News
        public ActionResult Index(int? page)
        {
            IEnumerable<tb_News> items = db.tb_News.OrderByDescending(x => x.id);
            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            //items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);
        }
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Add(tb_News model)
        {
            if (ModelState.IsValid)
            { 
                UserLogin userLogin = (UserLogin)Session["USER_SESSION"];
                model.CreateBy = db.tb_User.Find(userLogin.UserId).id; 
                model.CreateDate = DateTime.Now;
                model.ModifierDate = DateTime.Now;
                model.CategoryId = model.CategoryId;
                model.SeoKeywords = model.SeoKeywords;
                model.ModifierBy=model.ModifierBy;
                model.Position=model.Position;
                model.Detail = model.Detail;
                if (string.IsNullOrEmpty(model.Meta))
                    model.Meta = TiKayStore.Models.Common.Filter.FilterChar(model.Title);
                var hide = Request.Form.GetValues("hide")[0];
                if (hide == "true")
                {
                    model.Hide = true;
                }
                else
                {
                    model.Hide = false;
                }

                db.tb_News.Add(model);
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
            var i = db.tb_News.Find(id);
            return View(i);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(tb_News model)
        {
            if (ModelState.IsValid)
            {
                model.ModifierDate = DateTime.Now;
                model.Meta = model.Meta = TiKayStore.Models.Common.Filter.FilterChar(model.Title);
                db.tb_News.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.tb_News.Find(id);
            if (item != null)
            {
                db.tb_News.Remove(item);
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
                var item = db.tb_News.Find(id);
                if (item != null)
                {
                    db.tb_News.Remove(item);
                    db.SaveChanges();
                }
            }
            return Json(new { success = true });
        }
    }
}