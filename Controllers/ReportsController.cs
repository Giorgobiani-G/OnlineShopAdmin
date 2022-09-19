using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopAdmin.DataAccess.DbContexts;
using System.Threading.Tasks;
using System.Linq;
using OnlineShopAdmin.Models;
using System;
using System.Collections.Generic;
using OnlineShopAdmin.Filters;
using OnlineShopAdmin.DataAccess.Repository;
using OnlineShopAdmin.DataAccess.Models;
using OnlineShopAdmin.DataAccess;
using System.Threading;

namespace OnlineShopAdmin.Controllers
{
    [HttpRequestInfo]
    public class ReportsController : Controller
    {
        private readonly AdventureWorksLT2019Context _context;
        private readonly ReportingDA _reportingDA;
        public ReportsController(AdventureWorksLT2019Context context, ReportingDA reportingDA)
        {
            _context = context;
            _reportingDA = reportingDA;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ExecutionDuaration]
        public async Task<IActionResult> YearAndMonth(CancellationToken cancellationToken = default)
        {
            var list = await _reportingDA.YearAndMonthReport(cancellationToken);
            return View(list);
        }

        [ExecutionDuaration]
        public async Task<IActionResult> Products(CancellationToken cancellationToken = default)
        {
            var model = await _reportingDA.Products(cancellationToken);
            return View(model);
        }

        [ExecutionDuaration]
        public async Task<IActionResult> ProductCateGories(CancellationToken cancellationToken = default)
        {
            var model = await _reportingDA.ProductCateGories(cancellationToken);
            return View(model);
        }

        [ExecutionDuaration]
        public async Task<IActionResult> CustomersAndYears(CancellationToken cancellationToken = default)
        {
            var model = await _reportingDA.CustomersAndYears(cancellationToken);
            return View(model);
        }

        [ExecutionDuaration]
        public async Task<IActionResult> City(CancellationToken cancellationToken = default)
        {
            var model = await _reportingDA.City(cancellationToken);
            return View(model);
        }

        [ExecutionDuaration]
        public async Task<IActionResult> Top10CustomersBysales(CancellationToken cancellationToken = default)
        {
            var model = await _reportingDA.Top10CustomersBysales(cancellationToken);
            return View(model);
        }

        [ExecutionDuaration]
        public async Task<IActionResult> Top10ProductsBySalesAmount(CancellationToken cancellationToken = default)
        {
            var model = await _reportingDA.Top10ProductsBySalesAmount(cancellationToken);
            return View(model);
        }


    }
}
