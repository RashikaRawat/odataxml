//------------------------------------------------------------------------------
// <copyright file="WebDataService.svc.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using System.Xml.Linq;

namespace WebApplication1
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class WcfDataService1 : DataService<MyDataSource>
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
            config.UseVerboseErrors = true;
        }
    }

    [DataServiceKey("CustomerID")]
    public class Customer
    {
        public string CustomerID { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }

 

    [DataServiceKey("OrderID")]
    public class Order
    {
        public int OrderID { get; set; }
        public string CustomerID { get; set; }
        public int? EmployeeID { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public Decimal? Freight { get; set; }
        public string ShipName { get; set; }
        public string ShipCity { get; set; }
        public string ShipCountry { get; set; }
        public Customer Customer { get; set; }
        public Employee Employee { get; set; }
    }

    [DataServiceKey("EmployeeID")]
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }

    public class MyDataSource
    {
        static MyDataSource()
        {
            _Orders =
                XElement.Load(@"C:\usertmp\XOrders.xml")
                .Elements("Order")
                .Select(x => new Order
                {
                    OrderID = (int)x.Element("OrderID"),
                    CustomerID = (string)x.Element("CustomerID"),
                    EmployeeID = string.IsNullOrEmpty((string)x.Element("EmployeeID")) ? null : (int?)x.Element("EmployeeID"),
                    OrderDate = string.IsNullOrEmpty((string)x.Element("OrderID")) ? null : (DateTime?)x.Element("OrderDate"),
                    ShippedDate = string.IsNullOrEmpty((string)x.Element("ShippedDate")) ? null : (DateTime?)x.Element("ShippedDate"),
                    Freight = string.IsNullOrEmpty((string)x.Element("Freight")) ? null : (Decimal?)x.Element("Freight"),
                    ShipName = (string)x.Element("ShipName"),
                    ShipCity = (string)x.Element("ShipCity"),
                    ShipCountry = (string)x.Element("ShipCountry"),

                }).ToArray();


           _Customers =
           XElement.Load(@"C:\usertmp\XCustomers.xml")
           .Elements("Customer")
           .Select(x => new Customer
           {
               CustomerID = (string)x.Element("CustomerID"),
               CompanyName = (string)x.Element("CompanyName"),
               ContactName = (string)x.Element("ContactName"),
           }).ToArray();


            _Employees =
            XElement.Load(@"C:\usertmp\XEmployees.xml")
            .Elements("Employee")
            .Select(x => new Employee
            {
                EmployeeID = (int)x.Element("EmployeeID"),
                FirstName = (string)x.Element("FirstName"),
                LastName = (string)x.Element("LastName"),
            }).ToArray();


           



            var _os = _Orders.ToLookup(o => o.CustomerID);
            var _cs = _Customers.ToDictionary(c => c.CustomerID);
            var _o = _Orders.ToLookup(e => e.EmployeeID);
            var _d = _Employees.ToDictionary(e => e.EmployeeID);

            foreach (var o in _Orders) o.Customer = _cs[o.CustomerID];
            foreach (var c in _Customers) c.Orders = _os[c.CustomerID];
            foreach (var oe in _Orders) oe.Employee = oe.EmployeeID == null ? null : _d[(int)oe.EmployeeID];
            foreach (var e in _Employees) e.Orders = _o[e.EmployeeID];
        }

        static IEnumerable<Customer> _Customers;
        static IEnumerable<Employee> _Employees;
        static IEnumerable<Order> _Orders;



        public IQueryable<Customer> Customers { get { return _Customers.AsQueryable(); } }
        public IQueryable<Employee> Employees { get { return _Employees.AsQueryable(); } }
        public IQueryable<Order> Orders { get { return _Orders.AsQueryable(); } }

    }

}
