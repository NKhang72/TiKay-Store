using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TiKayStore.Models;
using TiKayStore.Areas.Admin.Model;
namespace TiKayStore.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        // GET: Admin/Account
        PhoneStoreEntities1 db = new PhoneStoreEntities1();
        public ActionResult Index()
        {
            UserLogin userLogin = (UserLogin)Session["USER_SESSION"];
            if (userLogin != null)
            {
                return View(userLogin);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
        public ActionResult Edit(int id)
        {
            var item = db.tb_User.Find(id);
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(tb_User model)
        {
            if (ModelState.IsValid)
            {
                var item = db.tb_User.Find(model.id);
                if (TiKayStore.Models.Common.Encryption.GetHashMD5(model.password) != item.password)
                {
                    ModelState.AddModelError("", "Mật khẩu không đúng");
                    return View("Edit");
                }
                else if( model.password == null)
                {
                    ModelState.AddModelError("", "Vui lòng nhập mật khẩu");
                    return View("Edit");
                }
                else
                {
                    item.firstName = model.firstName;
                    item.lastName = model.lastName;
                    item.email = model.email;
                    item.image = model.image;
                    item.password = TiKayStore.Models.Common.Encryption.GetHashMD5(model.password);
                    item.status = true;
                    db.tb_User.Attach(item);
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    UserLogin userLogin = (UserLogin)Session["USER_SESSION"];
                    if (userLogin != null)
                    {
                        userLogin.FirstName = model.firstName;
                        userLogin.LastName = model.lastName;
                        userLogin.Email = model.email;
                        userLogin.Image = model.image;
                    }
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }
        public ActionResult EditPassword(int id)
        {
            var item = db.tb_User.Find(id);
            EditPasswordModel model = new EditPasswordModel();
            model.id = id;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditPassword(EditPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var item = db.tb_User.Find(model.id);
                if (TiKayStore.Models.Common.Encryption.GetHashMD5(model.passwordOld) != item.password)
                {
                    ModelState.AddModelError("", "Mật khẩu không đúng");
                    return View("EditPassword");
                }
                else
                {
                    if (model.passwordNew != model.passwordConfirm)
                    {
                        ModelState.AddModelError("", "Mật khẩu xác minh không đúng");
                        return View("EditPassword");
                    }
                    else
                    {
                        if (model.passwordNew.Length < 6)
                        {
                            ModelState.AddModelError("", "Mật khẩu ít nhất 6 ký tự");
                            return View("EditPassword");
                        }
                        else
                        {
                            item.password = TiKayStore.Models.Common.Encryption.GetHashMD5(model.passwordNew);
                            db.tb_User.Attach(item);
                            db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }

                    }
                }
            }
            return View(model);
        }
    }
}