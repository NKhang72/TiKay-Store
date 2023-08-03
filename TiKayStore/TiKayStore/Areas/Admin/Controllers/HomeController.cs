using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TiKayStore.Models;

namespace TiKayStore.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult gerUserName()
        {
            UserLogin userLogin = (UserLogin)Session["USER_SESSION"];
            if (userLogin != null)
            {
                return PartialView(userLogin);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
            
        }
    }
}