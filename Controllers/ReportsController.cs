using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopAdmin.DataAccess.DbContexts;
using System.Threading.Tasks;
using System.Linq;
using OnlineShopAdmin.Models;
using System;
using System.Collections.Generic;
using OnlineShopAdmin.Filters;

namespace OnlineShopAdmin.Controllers
{
    [HttpRequestInfo]
    public class ReportsController : Controller
    {
        private readonly AdventureWorksLT2019Context _context;

        public ReportsController(AdventureWorksLT2019Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ExecutionDuaration]
        public async Task<IActionResult> YearAndMonth()
        {

            var query = await _context.SalesOrderHeaders
                  .GroupBy(f => new { f.OrderDate.Year, f.OrderDate.Month }).OrderBy(x => x.Key.Year)
                   .Select(group => new { date = group.Key, total = group.Sum(f => f.TotalDue) }).ToListAsync();

            List<YearAndMonthViewModel> list = new();

            foreach (var item in query)
            {
                YearAndMonthViewModel som = new YearAndMonthViewModel();
                som.Year = item.date.Year;
                som.Month = item.date.Month;
                som.Sum = item.total;
                list.Add(som);
            }

            list[list.Count - 1].Dictionary = list.GroupBy(x => x.Year)
            .Select(group => new { year = group.Key, sum = group.Sum(f => f.Sum) }).ToDictionary(a => a.year, e => e.sum);

            return View(list);
        }

        [ExecutionDuaration]
        public IActionResult Products()
        {

            var groupJoin = (from T1 in _context.SalesOrderDetails
                             join T2 in _context.Products on T1.ProductId equals T2.ProductId

                             group T1 by new { T1.Product.Name } into g
                             select new
                             {
                                 name = g.Key.Name,

                                 Amount = g.Sum(t => t.LineTotal)
                             }).ToList();


            List<ProductsReportViewModel> model = new List<ProductsReportViewModel>();

            foreach (var item in groupJoin)
            {
                ProductsReportViewModel viewModel = new ProductsReportViewModel();
                viewModel.Name = item.name;
                viewModel.Sum = item.Amount;
                model.Add(viewModel);
            }

            return View(model);
        }

        [ExecutionDuaration]
        public IActionResult ProductCateGories()
        {

            var groupJoin = (from T1 in _context.SalesOrderDetails
                             join T2 in _context.Products on T1.ProductId equals T2.ProductId
                             join T3 in _context.ProductCategories on T2.ProductCategoryId equals T3.ProductCategoryId

                             group T1 by new { T2.ProductCategory.Name } into g
                             select new
                             {
                                 name = g.Key.Name,

                                 Amount = g.Sum(t => t.LineTotal)
                             }).ToList();


            List<ProductsReportViewModel> model = new List<ProductsReportViewModel>();

            foreach (var item in groupJoin)
            {
                ProductsReportViewModel viewModel = new ProductsReportViewModel();
                viewModel.Name = item.name;
                viewModel.Sum = item.Amount;
                model.Add(viewModel);
            }

            return View(model);
        }

        [ExecutionDuaration]
        public IActionResult CustomersandYears()
        {
            var groupJoin = (from T1 in _context.SalesOrderHeaders
                             join T2 in _context.Customers on T1.CustomerId equals T2.CustomerId


                             group T1 by new { T1.Customer.CustomerId, T1.Customer.FirstName, T1.OrderDate.Year } into g
                             select new
                             {
                                 name =  g.Key.FirstName,
                                 year = g.Key.Year,
                                 Amount = g.Sum(t => t.TotalDue)
                             }).ToList();

            List<CustomersAndYearsReportViewModel> model = new List<CustomersAndYearsReportViewModel>();

            foreach (var item in groupJoin)
            {
                CustomersAndYearsReportViewModel viewModel = new CustomersAndYearsReportViewModel();
                viewModel.Name = item.name;
                viewModel.Sum = item.Amount;
                viewModel.Year = item.year;
                model.Add(viewModel);
            }

            return View(model);
        }

        [ExecutionDuaration]
        public IActionResult City()
        {
            var groupJoin = (from T1 in _context.SalesOrderHeaders
                             join T2 in _context.Addresses on T1.ShipToAddressId equals T2.AddressId


                             group T1 by new { T2.City} into g
                             select new
                             {
                                 name = g.Key.City,
                                
                                 Amount = g.Sum(t => t.TotalDue)
                             }).ToList();

            List<ProductsReportViewModel> model = new List<ProductsReportViewModel>();

            foreach (var item in groupJoin)
            {
                ProductsReportViewModel viewModel = new ProductsReportViewModel();
                viewModel.Name = item.name;
                viewModel.Sum = item.Amount;
                model.Add(viewModel);
            }

            return View(model);
        }

        [ExecutionDuaration]
        public IActionResult Top10CustomersBysales()
        {
            var groupJoin = (from T1 in _context.SalesOrderHeaders
                             join T2 in _context.Customers on T1.CustomerId equals T2.CustomerId                  
                             group T1 by new { T1.Customer.CustomerId, T1.Customer.FirstName } into g
                             select new
                             {
                                 name = g.Key.FirstName,
                                 Amount = g.Sum(t => t.TotalDue)
                             }).OrderByDescending(x=>x.Amount).Take(10).ToList();

            List<ProductsReportViewModel> model = new List<ProductsReportViewModel>();

            foreach (var item in groupJoin)
            {
                ProductsReportViewModel viewModel = new ProductsReportViewModel();
                viewModel.Name = item.name;
                viewModel.Sum = item.Amount;
                model.Add(viewModel);
            }

            return View(model);
        }

        [ExecutionDuaration]
        public IActionResult Top10ProductsBySalesAmount()
        {

            var groupJoin = (from T1 in _context.SalesOrderDetails
                             join T2 in _context.Products on T1.ProductId equals T2.ProductId
                             //orderby T1.LineTotal descending
                             group T1 by new { T1.Product.Name } into g
                             select new
                             {
                                 name = g.Key.Name,

                                 Amount = g.Sum(t => t.LineTotal)
                             }).OrderByDescending(x=>x.Amount).Take(10).ToList();


            List<ProductsReportViewModel> model = new List<ProductsReportViewModel>();

            foreach (var item in groupJoin)
            {
                ProductsReportViewModel viewModel = new ProductsReportViewModel();
                viewModel.Name = item.name;
                viewModel.Sum = item.Amount;
                model.Add(viewModel);
            }

            return View(model);
        }


    }
}
