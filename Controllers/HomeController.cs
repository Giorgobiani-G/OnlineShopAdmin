using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineShopAdmin.DataAccess;
using OnlineShopAdmin.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopAdmin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductsDA productsDA;

        public HomeController(ILogger<HomeController> logger, IProductsDA productsDA)
        {
            _logger = logger;
            this.productsDA = productsDA;
        }

        public IActionResult Index()
        {
            var result = productsDA.GetProducts().ToList();
            return View(result);
        }

    }
}
