using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLySieuThi.DTO;
using QuanLySieuThi.BUS;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using QuanLySieuThi.Filter;

namespace QuanLySieuThi.Controllers
{
    [CommonAttributeFilter]
    public class CartController : Controller
    {
        private List<BillDetail> countQuality
        {
            get
            {
                var countQuality = Session["countQuality"] as List<BillDetail>;
                if (countQuality == null)
                {
                    countQuality = new List<BillDetail>();
                    Session["countQuality"] = countQuality;
                }
                return countQuality;
            }
            set
            {
                Session["countQuality"] = value;
            }
        }
        private List<BillDetail> Cart
        {
            get
            {
                var cart = Session["Cart"] as List<BillDetail>;
                if (cart == null)
                {
                    cart = new List<BillDetail>();
                    Session["Cart"] = cart;
                }
                return cart;
            }
            set
            {
                Session["Cart"] = value;
            }
        }

        // GET: Cart
        public ActionResult Index()
        {
            var cart = this.Cart;

            ViewBag.Cart = cart;

            return View();
        }
        /* -------phương thức đếm số lượng sản phẩm trong giỏ--------------*/
        public ActionResult Count()
        {
            var cart = this.Cart;
            var cartCount = cart.Sum(BD => BD.Quantity);

            return Json(new { cartCount }, JsonRequestBehavior.AllowGet);
        }

        /* -------phương thức khi thêm sản phẩm vào trong giỏ--------------*/
        [HttpPost]
        public ActionResult AddCart()
        {

            var cart = this.Cart;

            using (var reader = new StreamReader(Request.GetBufferlessInputStream()))
            {
                var body = reader.ReadToEnd();
                var data = JsonConvert.DeserializeObject<dynamic>(body);

                var countQuality = this.countQuality;

                int productId = data.productId;
                int productQuality = data.productQuality;
                decimal productPrice = data.productPrice;

                var existingBillDetail = cart.FirstOrDefault(BD => BD.ProductID == productId);

                if (existingBillDetail != null)
                {
                    // Lấy ra đúng sản phẩm đã mua theo ID
                    var existingCountDetail = countQuality.FirstOrDefault(BD => BD.ID == productId);

                    // Kiểm tra sản phẩm này đã được đặt chưa
                    if (existingCountDetail != null)
                    {
                        //cộng dồn số lần người dùng thêm sản phẩm
                        existingCountDetail.Quantity += productQuality;

                        // Kiểm tra số lượng mua với unitstock của sản phẩm
                        if (existingCountDetail.Quantity <= existingBillDetail.Product.UnitInStock)
                        {
                            // Nếu sản phẩm đã tồn tại, cập nhật trường quality của sản phẩm đó
                            existingBillDetail.Quantity += productQuality;
                        }
                        else
                        {
                            existingCountDetail.Quantity = existingBillDetail.Quantity;
                            ViewBag.FailMsg = "Bạn không được mua quá lượng tồn kho của sản phẩm";
                            return RedirectToAction("Product", "Product", new { id = productId });

                        }

                    }

                }
                else
                {
                    ProductBUS bus = new ProductBUS();
                    Product p = new Product();
                    p = bus.GetProduct(productId);

                    BillDetail newBillDetail = new BillDetail(productQuality, productPrice, productId, p);

                    BillDetail newQuality = new BillDetail(productId, productQuality);


                    cart.Add(newBillDetail);
                    countQuality.Add(newQuality);
                }
                Session["countQuality"] = countQuality;
                Session["Cart"] = cart;
            }

            return View();
        }

        /* -------phương thức khi thanh toán sản phẩm trong giỏ--------------*/
        [AuthenticationFilter]
        [HttpPost]
        public ActionResult Checkout(string inputname)
        {
            
            //khai báo
            BillBUS billBUS = new BillBUS();
            ProductBUS productBUS = new ProductBUS();

            // Lấy giỏ hàng từ Session
            var cart = this.Cart;
            DateTime DateNow = DateTime.Now;

            // Tính tổng số tiền cần thanh toán
            decimal total = 0;
            foreach (var item in cart)
            {
                total += (decimal)item.Quantity * (decimal)item.Price;
            }
            decimal discountPercent = Math.Min(decimal.Parse(inputname) / 1000, 0.4m);


            decimal discountAmount = total * discountPercent;

            decimal finalAmount = total - discountAmount;

            var currentUser = Session["currentUser"] as Customer;
            // Lưu thông tin vào cơ sở dữ liệu
            int bonus = (int)(total / 1000);
            if (currentUser != null)
            {
                Bill bill = new Bill() { CreatedDate = DateTime.Now, SubTotal = finalAmount, CustomerID = currentUser.ID };
                billBUS.Create(bill, cart);

                CustomerBUS customerBUS = new CustomerBUS(); 
               
            }



            // Xóa giỏ hàng khỏi Session
            Session.Remove("Cart");
            return View();
       }

        /* -------phương thức khi xoá sản phẩm trong giỏ------------*/
        public ActionResult DeleteCart(int productId)
        {
            var cart = this.Cart;
            var existingBillDetail = cart.FirstOrDefault(BD => BD.ProductID == productId);

            if (existingBillDetail != null)
            {
                // Nếu sản phẩm đã tồn tại trong giỏ hàng, tiến hành xóa
                cart.Remove(existingBillDetail);
                Session["Cart"] = cart; // Lưu giỏ hàng mới vào session
            }

            return RedirectToAction("Index");
        }

        /* -------phương thức update sản phẩm trong giỏ------------*/

        [HttpPost]
        public ActionResult UpdateCart()
        {
            var cart = this.Cart;

            using (var reader = new StreamReader(Request.GetBufferlessInputStream()))
            {
                var body = reader.ReadToEnd();
                var data = JsonConvert.DeserializeObject<dynamic>(body);

                Console.WriteLine(data.cartItems);

                BillDetail[] cartItems = JsonConvert.DeserializeObject<BillDetail[]>(data.cartItems.ToString());

                foreach (var item in cartItems)
                {
                    var existingBillDetail = cart.FirstOrDefault(BD => BD.ProductID == item.ID);
                    if (existingBillDetail != null)
                    {
                        existingBillDetail.Quantity = item.Quantity;
                    }

                }
                Session["Cart"] = cart;
            }

            return RedirectToAction("Index");
        }

    }
}