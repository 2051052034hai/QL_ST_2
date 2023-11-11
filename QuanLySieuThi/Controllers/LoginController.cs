using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLySieuThi.DTO;
using QuanLySieuThi.BUS;
using System.Web.Helpers;
using QuanLySieuThi.Filter;
using QuanLySieuThi.Utils;

namespace QuanLySieuThi.Controllers
{
    [CommonAttributeFilter]
    public class LoginController : Controller
    {
        // GET: Login
        [AnonymousFilter]
        public ActionResult Index()
        {
            ViewBag.FailMsg = TempData["FailMsg"] as string;
            ViewBag.Username = TempData["username"] as string;
            return View();
        }

        [AnonymousFilter]
        public ActionResult Login()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        [AnonymousFilter]
        public ActionResult Login(string loginType, string username, string password)
        {
           
                CustomerBUS customerBUS = new CustomerBUS();
                Customer customer = customerBUS.Authenticate(username, Utils.Utils.GetMD5(password));
                if (customer != null)
                {
                    Session["currentUser"] = customer;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["FailMsg"] = "Đăng nhập thất bại!!! Username hoặc password không khớp";
                    TempData["username"] = username;
                    return RedirectToAction("Index", "Login");
                }
           
            //// Authenticate the user here using the provided username and password
            //if (true)
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            //else
            //{
            //    // If authentication fails, display an error message
            //    ViewBag.Error = "Invalid username or password. Please try again.";
            //    getAllCategories();
            //    return View();
            //}
        }

        [AuthenticationFilter]
        public ActionResult Logout()
        {
            Session.Remove("currentUser");
            return RedirectToAction("Login", "Login");
        }

        [HttpGet]
        public ViewResult Register()
        {
            ViewBag.FailMsg = TempData["FailMsg"] as string;
            return View();
        }
        
        
        [HttpPost]
        [AnonymousFilter]
        public ActionResult AddCustomer()
        {
            // Dữ liệu nhận được từ client fetch lên
            string username = Request.Form["username"];
            string fullname = Request.Form["fullname"];
            string phone = Request.Form["phone"];
            string adress = Request.Form["address"];
            string password = Request.Form["password"];

            Customer cus = new Customer(fullname, adress, phone, Utils.Utils.GetMD5(password), username);
            
            CustomerBUS customerBUS = new CustomerBUS();
            if (customerBUS.Create(cus) > 0)
                return RedirectToAction("Login", "Login");
            else
            {
                TempData["FailMsg"] = "Đăng ký không thành công!!! Tài khoản đã có trong hệ thống";
                return RedirectToAction("Register", "Login");
            }     
        }

    }
}