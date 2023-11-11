using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLySieuThi.DTO;
using QuanLySieuThi.BUS;
using QuanLySieuThi.Filter;

namespace QuanLySieuThi.Controllers
{
    [CommonAttributeFilter]
    [AuthenticationFilter]
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            Customer customer = Session["currentUser"] as Customer;
            ViewBag.Customer = customer;
            ViewBag.SuccessMsg = TempData["SuccessMsg"] as string;
            return View();
        }
        [HttpGet]
        public ActionResult ChangePassword()
        {
            Customer cus = Session["currentUser"] as Customer;
            ViewBag.Customer = cus;
            ViewBag.SuccessMsg = TempData["SuccessMsg"] as string;
            ViewBag.FailMsg = TempData["FailMsg"] as string;
            return View();
        }

        [HttpPost]
        public ActionResult Edit()
        {
            string ID = Request.Form["ID"];
            string fullname = Request.Form["fullname"];
            string phone = Request.Form["phone"];
            string address = Request.Form["address"];
            string password = Request.Form["password"];
            CustomerBUS customerBUS = new CustomerBUS();
            if (customerBUS.Update(ID, fullname, phone, address, password) > 0)
            {
                TempData["SuccessMsg"] = "Cập nhật thành công!!!";
                Session["currentUser"] = customerBUS.GetCustomerById(Int32.Parse(ID));
            }
            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        public ActionResult Change()
        {
            string ID = Request.Form["ID"];
            string password = Request.Form["password"];
            string confirm = Request.Form["confirm"];
            string newPass = Request.Form["newPassword"];
            CustomerBUS customerBUS = new CustomerBUS();
            if (customerBUS.Update(ID: ID, password: Utils.Utils.GetMD5(password), newPass: Utils.Utils.GetMD5(newPass)) > 0)
            {
                TempData["SuccessMsg"] = "Cập nhật thành công!!!";
                Session["currentUser"] = customerBUS.GetCustomerById(Int32.Parse(ID));
            }
            else
                TempData["FailMsg"] = "Cập nhật thất bại!!!";
            return Json(new { success = true });

        }
    }
}