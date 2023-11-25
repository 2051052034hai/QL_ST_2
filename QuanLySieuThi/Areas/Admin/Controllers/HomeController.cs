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
            ViewBag.SuccessMsg = TempData["SuccessMsg"];
            BillBUS billBus = new BillBUS();
            List<Bill> listbill = billBus.GetAll().OrderByDescending(x => x.ID).ToList();
            ViewBag.ListBill = listbill;
            ProductBUS bus = new ProductBUS();
            // Lấy ra danh sách sản phẩm có trường deleteFlg bằng 0 hoặc null
            var products = bus.GetProducts().Where(p => p.deleteFlg == 0).ToList();
            ViewBag.Products = products;
            return View();
        }
    }
}