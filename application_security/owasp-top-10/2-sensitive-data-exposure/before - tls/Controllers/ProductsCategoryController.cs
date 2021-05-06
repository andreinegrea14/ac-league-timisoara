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

            //Needed for EF
             _dbContext = dbContext;
        }

        public IActionResult Categories()
        {
            //With EF
            var products = _dbContext.ProductSubcategories.Select(
                product=> new ProductCategory {
                            Name =product.Name,
                            CategoryId = product.ProductSubcategoryId.ToString()
                        }).ToList();

            return View(products);
        }

        public IActionResult Products(int productCategory)
        {
            var products = _dbContext.Products.Where(product=> product.ProductSubcategoryId == productCategory).
            Select(product => new Product {
                            Name =product.Name,
                            ProductNumber = product.ProductNumber,
                            Price = product.ListPrice.ToString()
                        }).ToList();


            return View(products);
        }
    }
}