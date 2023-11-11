using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLySieuThi.DTO;
using QuanLySieuThi.BUS;
namespace QuanLySieuThi.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            BillBUS billBus = new BillBUS();
            List<Bill> listbill = billBus.GetAll().OrderByDescending(x => x.ID).ToList();
            ViewBag.ListBill = listbill;
            ProductBUS bus = new ProductBUS();
            ViewBag.Products = bus.GetProducts();
            return View();
        }
    }
}