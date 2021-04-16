using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Injection.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Injection.Controllers
{
    public class ProductsCategoryController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private string _connectionString;

        public ProductsCategoryController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;

            _connectionString = configuration.GetConnectionString("MvcSampleDbContext");

            //test extension-  Insecure hashing algorithm
            //var md5 = new MD5CryptoServiceProvider();
        }

        public IActionResult Categories()
        {
            var products = new List<ProductCategory>();
             var sqlString = "SELECT * FROM Production.ProductSubcategory ORDER BY Name";
            using (var conn = new SqlConnection(_connectionString))
            { 
                using (var command = new SqlCommand(sqlString, conn))
                {
                    command.Connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        products.Add(new ProductCategory {
                            Name =reader.GetValue(reader.GetOrdinal("name")).ToString(),
                            CategoryId = reader.GetValue(reader.GetOrdinal("ProductSubCategoryID")).ToString()
                        });
                    }
                }
            }
            return View(products);
        }

        public IActionResult Products(string productCategory)
        {
            var products = new List<Product>();
            var sqlString = "SELECT * FROM Production.Product WHERE ProductSubCategoryID = " + productCategory;
            using (var conn = new SqlConnection(_connectionString))
            { 
                using (var command = new SqlCommand(sqlString, conn))
                {
                    command.Connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        products.Add(new Product {
                            Name =reader.GetValue(reader.GetOrdinal("name")).ToString(),
                            ProductNumber = reader.GetValue(reader.GetOrdinal("productnumber")).ToString(),
                            Price = reader.GetValue(reader.GetOrdinal("listprice")).ToString()
                        });
                    }
                 
                }
            }
            return View(products);
        }
    }
}