using QuanLySieuThi.DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace QuanLySieuThi.DAO
{
    public class CustomerDAO
    {
        private QuanLySieuThiContext context;

        public CustomerDAO()
        {
            this.context = new QuanLySieuThiContext();
        }
        public List<Customer> GetAllCustomers()
        {
            return context.Customers.ToList();
        }

        public int Create(Customer customer)
        {
            try
            {
                context.Customers.Add(customer);
                return context.SaveChanges();
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
                return 0;
            }
        }

        // Read a customer by ID
        public Customer GetCustomerById(int customerId)
        {
            return context.Customers.Find(customerId);
        }
        public Customer ViewDetail(int id)
        {
            return context.Customers.Find(id);
        }
        // Update an existing customer
        public int Update(Customer customer)
        {
            try
            {
                context.Entry(customer).State = EntityState.Modified;
                return context.SaveChanges();
            }
            catch
            {
                return 0;
            }
        }

        public int Delete(int customerId)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Customer customer = GetCustomerById(customerId);
                    if (customer != null)
                    {
                        List<Bill> bills = context.Bills.Where(bill => bill.CustomerID == customerId).ToList();
                        foreach (var bill in bills)
                        {
                            bill.CustomerID = null;
                            context.Entry(bill).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                    }
                    context.Customers.Remove(customer);
                    transaction.Commit();
                    return context.SaveChanges();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return 0;
                }
            }
        }


        public Customer GetByUsername(string username)
        {
            return context.Customers.FirstOrDefault(c => c.UserName == username);
        }
    }
}
