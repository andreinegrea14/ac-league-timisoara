using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Injection.Models;
using dbModels = Injection.DbModels;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;

namespace Injection.Controllers
{
    public class ProductsCategoryController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private string _connectionString;
        private dbModels.AdventureWorks2019Context _dbContext;

        public ProductsCategoryController(ILogger<HomeController> logger, IConfiguration configuration, dbModels.AdventureWorks2019Context dbContext)
        {
            _logger = logger;

            //Needed for ADO.NET
            _connectionString = configuration.GetConnectionString("MvcSampleDbContext");

            //Needed for EF
             _dbContext = dbContext;

            //test DevSkim extension-  Insecure hashing algorithm
            //var md5 = new MD5CryptoServiceProvider();
        }

        public IActionResult Categories()
        {
            var products = new List<ProductCategory>();

            //With ADO.NET
            // var sqlString = "SELECT * FROM Production.ProductSubcategory ORDER BY Name";
            // using (var conn = new SqlConnection(_connectionString))
            // { 
            //     using (var command = new SqlCommand(sqlString, conn))
            //     {
            //         command.Connection.Open();
            //         var reader = command.ExecuteReader();
            //         while (reader.Read())
            //         {
            //             products.Add(new ProductCategory {
            //                 Name =reader.GetValue(reader.GetOrdinal("name")).ToString(),
            //                 CategoryId = reader.GetValue(reader.GetOrdinal("ProductSubCategoryID")).ToString()
            //             });
            //         }
            //     }
            // }

            //With EF
             products = _dbContext.ProductSubcategories.Select(
                product=> new ProductCategory {
                            Name =product.Name,
                            CategoryId = product.ProductSubcategoryId.ToString()
                        }).ToList();

            return View(products);
        }

        public IActionResult Products(int productCategory)
        {
            //Input validation here or when possible directly in the method signature
            // if(!int.TryParse(productCategory, out int pid))
            //      throw new ApplicationException("invalid id");

            var products = new List<Product>();
            //With ADO.NET & Parameterized query
            // var sqlString = "SELECT * FROM Production.Product WHERE ProductSubCategoryID = @prodCat";
            // using (var conn = new SqlConnection(_connectionString))
            // { 
            //     using (var command = new SqlCommand(sqlString, conn))
            //     {
            //        command.Parameters.Add("@prodCat",SqlDbType.Int);
            //        command.Parameters["@prodCat"].Value = productCategory;
                    
            //         command.Connection.Open();
            //         var reader = command.ExecuteReader();
                    
            //         while (reader.Read())
            //         {
            //             products.Add(new Product {
            //                 Name =reader.GetValue(reader.GetOrdinal("name")).ToString(),
            //                 ProductNumber = reader.GetValue(reader.GetOrdinal("productnumber")).ToString(),
            //                 Price = reader.GetValue(reader.GetOrdinal("listprice")).ToString()
            //             });
            //         }
                 
            //     }
            // }

            /*  
                Using EF 
            */
            products = _dbContext.Products.Where(product=> product.ProductSubcategoryId == productCategory).
            Select(product => new Product {
                            Name =product.Name,
                            ProductNumber = product.ProductNumber,
                            Price = product.ListPrice.ToString()
                        }).ToList();


            return View(products);
        }
    }
}