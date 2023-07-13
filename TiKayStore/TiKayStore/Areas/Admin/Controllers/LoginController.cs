using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TiKayStore.Areas.Admin.Model;
using TiKayStore.Models;
using PagedList;

namespace TiKayStore.Areas.Admin.Controllers
{
    
        public class LoginController : Controller
        {

            PhoneStoreEntities1 db = new PhoneStoreEntities1();
            // GET: Admin/Login
            public ActionResult Index()
            {
                return View();
            }
            public ActionResult Login(LoginModel model)
            {
                if (ModelState.IsValid)
                {
                    var dao = new User_DAO();
                    var result = dao.Login(model.email, TiKayStore.Models.Common.Encryption.GetHashMD5(model.password));
                    if (result == 1)
                    {
                        var user = dao.getItem(model.email);
                        UserLogin session = new UserLogin();
                        session.Email = user.email;
                        session.UserId = user.id;
                        session.LastName = user.lastName;
                        session.FirstName = user.firstName;
                        session.Image = user.image;
                        Session["USER_SESSION"] = session;
                        return RedirectToAction("Index", "Products");

                    }
                    else if (result == 0)
                    {
                        ModelState.AddModelError("", "Tài khoản chưa được kích hoạt");
                    }
                    else if (result == -3)
                    {
                        ModelState.AddModelError("", "Mật khẩu chưa chính xác");
                    }
                    else if (result == -2)
                    {
                        ModelState.AddModelError("", "Tài khoản không tồn tại");
                    }
                    else if (result == -4)
                    {
                        ModelState.AddModelError("", "Mật khẩu phải ít nhất 6 ký tự");
                    }

            }
                return View("Index");
            }
            public ActionResult ListUser(int? page)
            {
                IEnumerable<tb_User> items = db.tb_User.OrderByDescending(x => x.id);
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
                return View();
            }
            [HttpPost]
            [ValidateAntiForgeryToken]
            [ValidateInput(false)]
            public ActionResult Add(tb_User model, string confirmPassword)
            {
                if (ModelState.IsValid)
                {
                    String email = model.email;
                    var dao = new User_DAO();
                    List<tb_User> users = dao.getList();
                    foreach (tb_User user in users)
                    {
                        if (user.email == email)
                        {
                            ModelState.AddModelError("", "Tài khoản da tồn tại");
                            return View("Add");
                        }
                        else
                        {
                            if (model.password == confirmPassword)
                            {
                                if (model.password.Length < 6)
                                {
                                    ModelState.AddModelError("", "Mật khẩu ít nhất 6 ký tự");
                                    return View("Add");
                                }
                                else
                                {
                                    model.password = TiKayStore.Models.Common.Encryption.GetHashMD5(model.password);
                                    var status = Request.Form.GetValues("status")[0];
                                    if (status == "true")
                                    {
                                        model.status = true;
                                    }
                                    else
                                    {
                                        model.status = false;
                                    }
                                    db.tb_User.Add(model);
                                    db.SaveChanges();
                                    return RedirectToAction("ListUser");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "Vui lòng nhập lại mật khẩu");
                                return View("Add");
                            }

                        }
                    }
                }

                return View(model);
            }
            [HttpPost]
            public ActionResult Delete(int id)
            {
                var item = db.tb_User.Find(id);
                if (item != null)
                {

                    db.tb_User.Remove(item);
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
                    var item = db.tb_User.Find(id);
                    if (item != null)
                    {
                        db.tb_User.Remove(item);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            public ActionResult IsActive(int id)
            {
                var item = db.tb_User.Find(id);
                Boolean status = item.status.Value;
                if (item != null)
                {
                    db.tb_User.Attach(item);
                    if (status == true)
                    {
                        item.status = false;
                    }
                    else
                    {
                        item.status = true;

                    }

                    db.Entry(item).Property(x => x.status).IsModified = true;
                    db.SaveChanges();
                    return Json(new { message = "Success", Success = true, isAcive = true });
                }
                return Json(new { success = false });

            }
        }
    
}