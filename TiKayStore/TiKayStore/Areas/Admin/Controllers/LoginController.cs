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
        public ActionResult Logout()
        {
            return View("~/Views/Home/Index.cshtml");
        }
        public static int CountOder()
        {
            PhoneStoreEntities1 db = new PhoneStoreEntities1();
            int count= db.tb_Order.Where(x => x.isRead==false).Count();
            return count;
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
                    return RedirectToAction("Sort", "Products");

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
            UserLogin user = (UserLogin)Session["USER_SESSION"];
            var count = db.tb_Indentify.Count(x => x.IdUser == user.UserId & x.IdPermission == 1);
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
        public void deleteIndentify(int idUser)
        {
            List<tb_Indentify> listUser = db.tb_Indentify.Where(x => x.IdUser == idUser).ToList();
            foreach (tb_Indentify t in listUser)
            {
                var item = db.tb_Indentify.Find(t.Id);
                if (item != null)
                {
                    db.tb_Indentify.Remove(item);
                    db.SaveChanges();
                }
            }
        }
        
        [HttpPost]
        public ActionResult Delete(int id)
        {
            UserLogin user = (UserLogin)Session["USER_SESSION"];
            var count = db.tb_Indentify.Count(x => x.IdUser == user.UserId & x.IdPermission == 2);
            if (count > 0)
            {
                var item = db.tb_User.Find(id);
                if (item != null)
                {
                    deleteIndentify(id);
                    db.tb_User.Remove(item);
                    db.SaveChanges();
                    return Json(new { success = true });
                }

                return Json(new { success = false });
            }
            else if (count == 0)
            {
                return Json(new { success = false });

            }
            return Json(new { success = false });

        }
        [HttpPost]
        public ActionResult deleteAll(String ids)
        {
            UserLogin user = (UserLogin)Session["USER_SESSION"];
            var count = db.tb_Indentify.Count(x => x.IdUser == user.UserId & x.IdPermission == 3);
            if (count > 0)
            {
                List<String> listIdString = ids.Split(',').ToList();
                List<int> listId = listIdString.Select(int.Parse).ToList();
                foreach (int id in listId)
                {
                    var item = db.tb_User.Find(id);
                    if (item != null)
                    {
                        deleteIndentify(id);
                        db.tb_User.Remove(item);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }

            return RedirectToAction("Unpermitted", "Error");

        }
        public ActionResult IsActive(int id)
        {
            UserLogin user = (UserLogin)Session["USER_SESSION"];
            var count = db.tb_Indentify.Count(x => x.IdUser == user.UserId & x.IdPermission == 4);
            if (count > 0)
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

            return RedirectToAction("Unpermitted", "Error");


        }
        public ActionResult Edit(int id)
        {
            List<tb_Indentify> user = db.tb_Indentify.Where(x => x.IdUser == id).ToList();

            List<CheckboxList_model> chkRetrieval = new List<CheckboxList_model>();
            List<tb_Permission> tb_Permissions = db.tb_Permission.ToList();
            foreach (var permission in tb_Permissions)
            {
                bool isChecked = false;

                foreach (var item in user)
                {
                    if (permission.Id == item.IdPermission)
                    {
                        isChecked = true;
                    }

                }
                chkRetrieval.Add(new CheckboxList_model { Text = permission.Permission, Value = permission.Id, Selected = isChecked });


                ViewBag.Retrieval = chkRetrieval;
            }


            var items = db.tb_User.Find(id);
            return View(items);
        }
        [HttpPost]
        public ActionResult Edit(String ids, String idUser, String Email, String lastName, String firstName)
        {
            int UserId = Int32.Parse(idUser);
            if (ids != "")
            {
                List<String> listIdString = ids.Split(',').ToList();
                List<int> listId = listIdString.Select(int.Parse).ToList();
                List<int> listPermission = db.tb_Indentify.Where(x => x.IdUser == UserId).Select(x => (int)x.IdPermission).ToList();
                List<int> add = new List<int>();
                List<int> delete = new List<int>();
                if (listPermission.Count == 0)
                {
                    add = listId;
                }
                else
                {
                    add = listId.Except(listPermission).ToList();
                    delete = listPermission.Except(listId).ToList();
                }

                if (add.Count != 0)
                {
                    addListIndentify(add, UserId);

                }
                if (delete.Count != 0)
                {
                    deleteListIndentify(delete, UserId);
                }

            }
            else
            {
                List<int> listPermission = db.tb_Indentify.Where(x => x.IdUser == UserId).Select(x => (int)x.IdPermission).ToList();
                deleteListIndentify(listPermission, UserId);
            }

            var user = db.tb_User.Find(UserId);
            user.email = Email;
            user.lastName = lastName;
            user.firstName = firstName;
            db.tb_User.Attach(user);
            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(new { success = true });
        }
        public void deleteListIndentify(List<int> listId, int idUser)
        {
            foreach(int id in listId)
            {
                var itemIndentidy = db.tb_Indentify.Where(x => x.IdPermission == id & x.IdUser == idUser).FirstOrDefault();
                db.tb_Indentify.Remove(itemIndentidy);
                db.SaveChanges();
            }
        }
        public void addListIndentify(List<int> listId, int idUser)
        {
            foreach (int id in listId)
            {
                tb_Indentify indentify=new tb_Indentify();
                indentify.IdPermission = id;
                indentify.IdUser = idUser;
                db.tb_Indentify.Add(indentify);
                db.SaveChanges();
            }
        }
    }

}