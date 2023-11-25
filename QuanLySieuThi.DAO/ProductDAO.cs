using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLySieuThi.DTO;

namespace QuanLySieuThi.DAO
{
    public class ProductDAO
    {
        private QuanLySieuThiContext context;
        public ProductDAO()
        {
            context = new QuanLySieuThiContext();
        }
        public List<Product> GetProducts()
        {
            List<Product> products = context.Products.ToList();
            return products;
        }
        
        public List<Product> GetProductsByCateID(int ID)
        {
            var query = context.Products.AsQueryable();
            List<Product> products = query.Where(p => p.CateID == ID).ToList();

            return products;
        }
        public Product ViewDetail(int id)
        {
            return context.Products.Find(id);
        }
        public List<Product> GetProducts(string keyword)
        {
            List<Product> products = context.Products.Where(p => p.Name.Contains(keyword)).ToList();
            return products;
        }
        public void DeleteProduct(int id)
        {
            Product product = context.Products.Find(id);

            if (product != null)
            {
                // Kiểm tra xem sản phẩm có liên kết với bất kỳ hóa đơn nào không
                if (!context.BillDetails.Any(oi => oi.ProductID == id))
                {
                    context.Products.Remove(product);
                    context.SaveChanges();
                }
                else
                {
                   
                    product.deleteFlg = 1;
                    context.SaveChanges();
                }
            }
        }
        public List<Product> GetProducts(Dictionary<string, string> queryParams)
        {
            var products = context.Products.AsQueryable();

            if (queryParams.ContainsKey("kw"))
            {
                string keyword = queryParams["kw"];
                products = products.Where(p => p.Name.Contains(keyword));
            }
            if (queryParams.ContainsKey("categoryId") && queryParams["categoryId"] != null)
            {
                int categoryId = int.Parse(queryParams["categoryId"]);
                products = products.Where(p => p.CateID == (int) categoryId);
            }

            return products.ToList();
        }
        public void Create(Product product)
        {
            try
            {
                context.Products.Add(product);
                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the validation errors
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the error messages together
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Throw a new exception with the full error message
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }
        public bool UpdateInfo(Product product, string Name, string UnitPrice, string UnitInStock, string CateID, string Description, string SuppilerID,string Image_Url)
        {
            try
            {
                product.Name = Name;
                product.UnitPrice = decimal.Parse(UnitPrice);
                product.UnitInStock = int.Parse(UnitInStock);
                product.CateID = int.Parse(CateID);
                product.Description = Description;
                product.SuppilerID = SuppilerID;
                product.Image_Url = Image_Url;
                context.Entry(product).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(Product product)
        {
            try
            {
                context.Entry(product).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public Product GetProductById(int id)
        {
            return context.Products.Find(id);
        }
        public Product AddProduct(Product product)
        {
            try
            {
                context.Products.Add(product);
                context.SaveChanges();
                return product;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
