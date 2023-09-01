using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using TiKayStore.Models;
using TiKayStore.Areas.Admin.Model;
using PagedList;

namespace TiKayStore.Areas.Admin.Controllers
{
    public class ShoppingCartController : Controller
    {
        PhoneStoreEntities1 db = new PhoneStoreEntities1();
        // GET: Admin/ShoppingCart
        public ActionResult Index(int? page, string typeSort, string currentFilter, string currentSortType, SearchOrder searchOrder)
        {
            ViewBag.CurrentSort = typeSort;

            ViewBag.Code = typeSort == "Code" ? "code_desc" : "Code";
            ViewBag.Name = typeSort == "Name" ? "name_desc" : "Name";
            ViewBag.Phone = typeSort == "Phone" ? "phone_desc" : "Phone";
            ViewBag.Money = typeSort == "Money" ? "money_desc" : "Money";
            ViewBag.State = typeSort == "State" ? "state_desc" : "State";
            ViewBag.IsRead = typeSort == "IsRead" ? "isRead_desc" : "IsRead";
            ViewBag.DateSort = String.IsNullOrEmpty(typeSort) ? "date_desc" : "";

            if (searchOrder.SearchString != null )
            {
                page = 1;
            }
            else
            {
                searchOrder.SearchString = currentFilter;
                searchOrder.SearchType= currentSortType;
                
            }

            ViewBag.CurrentFilter = searchOrder.SearchString;
            ViewBag.CurrentSortType = searchOrder.SearchType;

            IEnumerable<tb_Order> items = db.tb_Order;

            if (!String.IsNullOrEmpty(searchOrder.SearchString))
            {
                if (searchOrder.SearchType == "Số điện thoại")
                {
                    items = items.Where(x => x.Phone.Contains(searchOrder.SearchString));

                }
                else if (searchOrder.SearchType == "Mã đơn hàng")
                {
                    items = items.Where(x => x.Code.Contains(searchOrder.SearchString));

                }
                else
                {
                    items = items.Where(x => x.CustomerName.Contains(searchOrder.SearchString));

                }
            }
            

            switch (typeSort)
            {
                case "date_desc":
                    items = items.OrderBy(s => s.CreateDate);
                    break;

                case "Code":
                    items = items.OrderBy(s => s.Code);
                    break;
                case "code_desc":
                    items = items.OrderByDescending(s => s.Code);
                    break;
                case "IsRead":
                    items = items.OrderBy(s => s.isRead);
                    break;
                case "isRead_desc":
                    items = items.OrderByDescending(s => s.isRead);
                    break;

                case "Name":
                    items = items.OrderBy(s => s.CustomerName);
                    break;
                case "name_desc":
                    items = items.OrderByDescending(s => s.CustomerName);
                    break;

                case "Phone":
                    items = items.OrderBy(s => s.Phone);
                    break;
                case "phone_desc":
                    items = items.OrderByDescending(s => s.Phone);
                    break;

                case "State":
                    items = items.OrderBy(s => s.TypePay);
                    break;
                case "state_desc":
                    items = items.OrderByDescending(s => s.TypePay);
                    break;

                case "Money":
                    items = items.OrderBy(s => s.TotalAmount);
                    break;
                case "money_desc":
                    items = items.OrderByDescending(s => s.TotalAmount);
                    break;
                default:
                    items = items.OrderByDescending(s => s.CreateDate);
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
        [HttpPost]
        public ActionResult Update(int id)
        {
            var item = db.tb_Order.Find(id);
            String payment = item.TypePay;
            if (item != null)
            {
                db.tb_Order.Attach(item);
                if (payment == "1")
                {
                    item.TypePay = "2";
                }
                else
                {
                    item.TypePay = "1";

                }

                db.Entry(item).Property(x => x.TypePay).IsModified = true;
                db.SaveChanges();
                return Json(new { message = "Success", Success = true });
            }
            return Json(new { message = "Unsuccess", Success = false });
        }
        public ActionResult View(int id)
        {
            var item = db.tb_Order.Find(id);
            item.isRead= true;
            db.Entry(item).Property(x => x.isRead).IsModified = true;
            db.SaveChanges();
            return View(item);
        }
        public ActionResult SearchPartialView()
        {
           
            return PartialView();
        }
        public ActionResult Search(int? page, SearchOrder searchOrder)
        {
            if (ModelState.IsValid) {
                if (searchOrder.SearchString == null)
                {
                    ModelState.AddModelError("", "Tài khoản chưa được kích hoạt");
                }
                else
                {
                    IEnumerable<tb_Order> items;
                    if (searchOrder.SearchType == "Số điện thoại")
                    {
                        items = db.tb_Order.Where(x => x.Phone.Contains(searchOrder.SearchString)).OrderBy(x => x.id);

                    }
                    else if (searchOrder.SearchType == "Mã đơn hàng")
                    {
                        items = db.tb_Order.Where(x => x.Code.Contains(searchOrder.SearchString)).OrderBy(x => x.id);

                    }
                    else
                    {
                        items = db.tb_Order.Where(x => x.CustomerName.Contains(searchOrder.SearchString)).OrderBy(x => x.id);

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
            }
            return View("Index");



        }
    }
}