using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLySieuThi.DTO;
using QuanLySieuThi.DAO;

namespace QuanLySieuThi.BUS
{
    public class CustomerBUS
    {
        private CustomerDAO customerDAO;
        public CustomerBUS()
        {
            customerDAO = new CustomerDAO();
        }

        public Customer GetCustomerById(int id)
        {
            return customerDAO.GetCustomerById(id);
        }

        public List<Customer> GetCustomers()
        {
            return customerDAO.GetAllCustomers();
        }

        public int Create(Customer customer)
        {
            if (customerDAO.GetByUsername(customer.UserName) != null)
                return 0;
            return customerDAO.Create(customer);
        }
        public int Update(String ID, String fullname, String phone, String address, String password)
        {
            CustomerDAO customerDAO = new CustomerDAO();
            Customer customer = customerDAO.GetCustomerById(int.Parse(ID));
            customer.Name = fullname;
            customer.Phone = phone;
            customer.Address = address; 
            customer.Password = password;
            
            if (customerDAO.Update(customer) > 0)
                return 1;
            return 0;
        }
        public int Update(string ID, string password, string newPass)
        {
            CustomerDAO customerDAO = new CustomerDAO();
            Customer customer = customerDAO.GetCustomerById(int.Parse(ID));
            if (!customer.Password.Equals(password)) {
                return 0;
            }
            customer.Password = newPass;
            if (customerDAO.Update(customer) > 0)
                return 1;
            return 0;
        }

        public int Update(int ID)
        {
            CustomerDAO customerDAO = new CustomerDAO();
            Customer customer = customerDAO.GetCustomerById(ID);
            return customerDAO.Update(customer);
        }
        public Customer Authenticate(string username, string password)
        {
            Customer customer = customerDAO.GetByUsername(username);
            if (customer == null || !customer.Password.Equals(password))
                return null;
            return customer;
        }

        public int Delete(int ID)
        {
            return customerDAO.Delete(ID);
        }
    }
}
