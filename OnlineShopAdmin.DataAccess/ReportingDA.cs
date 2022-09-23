using Microsoft.EntityFrameworkCore;
using OnlineShopAdmin.DataAccess.DbContexts;
using OnlineShopAdmin.DataAccess.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopAdmin.DataAccess
{
    public class ReportingDA
    {
        private readonly AdventureWorksLT2019Context _context;

        public ReportingDA(AdventureWorksLT2019Context context)
        {
            _context = context;
        }

        public async Task<List<YearAndMonthViewModel>> YearAndMonthReport(CancellationToken cancellationToken = default)
        {
            var query = await _context.SalesOrderHeaders
                  .GroupBy(f => new { f.OrderDate.Year, f.OrderDate.Month }).OrderBy(x => x.Key.Year)
                   .Select(group => new { date = group.Key, total = group.Sum(f => f.TotalDue) }).ToListAsync(cancellationToken);

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

            return list;
        }

        public async Task<List<ProductsReportViewModel>> Products(CancellationToken cancellationToken = default)
        {
            var groupJoin = await (from T1 in _context.SalesOrderDetails
                                   join T2 in _context.Products on T1.ProductId equals T2.ProductId

                                   group T1 by new { T1.Product.Name } into g
                                   select new
                                   {
                                       name = g.Key.Name,

                                       Amount = g.Sum(t => t.LineTotal)
                                   }).ToListAsync(cancellationToken);


            List<ProductsReportViewModel> model = new List<ProductsReportViewModel>();

            foreach (var item in groupJoin)
            {
                ProductsReportViewModel viewModel = new ProductsReportViewModel();
                viewModel.Name = item.name;
                viewModel.Sum = item.Amount;
                model.Add(viewModel);
            }
            return model;
        }

        public async Task<List<ProductsReportViewModel>> ProductCateGories(CancellationToken cancellationToken = default)
        {
            var groupJoin = await (from T1 in _context.SalesOrderDetails
                                   join T2 in _context.Products on T1.ProductId equals T2.ProductId
                                   join T3 in _context.ProductCategories on T2.ProductCategoryId equals T3.ProductCategoryId

                                   group T1 by new { T2.ProductCategory.Name } into g
                                   select new
                                   {
                                       name = g.Key.Name,

                                       Amount = g.Sum(t => t.LineTotal)
                                   }).ToListAsync(cancellationToken);


            List<ProductsReportViewModel> model = new List<ProductsReportViewModel>();

            foreach (var item in groupJoin)
            {
                ProductsReportViewModel viewModel = new ProductsReportViewModel();
                viewModel.Name = item.name;
                viewModel.Sum = item.Amount;
                model.Add(viewModel);
            }
            return model;
        }

        public async Task<List<CustomersAndYearsReportViewModel>> CustomersAndYears(CancellationToken cancellationToken = default)
        {
            var groupJoin = await (from T1 in _context.SalesOrderHeaders
                                   join T2 in _context.Customers on T1.CustomerId equals T2.CustomerId


                                   group T1 by new { T1.Customer.CustomerId, T1.Customer.FirstName, T1.OrderDate.Year } into g
                                   select new
                                   {
                                       name = g.Key.FirstName,
                                       year = g.Key.Year,
                                       Amount = g.Sum(t => t.TotalDue)
                                   }).ToListAsync(cancellationToken);

            List<CustomersAndYearsReportViewModel> model = new List<CustomersAndYearsReportViewModel>();

            foreach (var item in groupJoin)
            {
                CustomersAndYearsReportViewModel viewModel = new CustomersAndYearsReportViewModel();
                viewModel.Name = item.name;
                viewModel.Sum = item.Amount;
                viewModel.Year = item.year;
                model.Add(viewModel);
            }

            return model;
        }

        public async Task<List<ProductsReportViewModel>> City(CancellationToken cancellationToken = default)
        {
            var groupJoin = await (from T1 in _context.SalesOrderHeaders
                                   join T2 in _context.Addresses on T1.ShipToAddressId equals T2.AddressId


                                   group T1 by new { T2.City } into g
                                   select new
                                   {
                                       name = g.Key.City,

                                       Amount = g.Sum(t => t.TotalDue)
                                   }).ToListAsync(cancellationToken);

            List<ProductsReportViewModel> model = new();

            foreach (var item in groupJoin)
            {
                ProductsReportViewModel viewModel = new();
                viewModel.Name = item.name;
                viewModel.Sum = item.Amount;
                model.Add(viewModel);
            }
            return model;
        }

        public async Task<List<ProductsReportViewModel>> Top10CustomersBysales(CancellationToken cancellationToken = default)
        {
            var groupJoin = await (from T1 in _context.SalesOrderHeaders
                                   join T2 in _context.Customers on T1.CustomerId equals T2.CustomerId
                                   group T1 by new { T1.Customer.CustomerId, T1.Customer.FirstName } into g
                                   select new
                                   {
                                       name = g.Key.FirstName,
                                       Amount = g.Sum(t => t.TotalDue)
                                   }).OrderByDescending(x => x.Amount).Take(10).ToListAsync(cancellationToken);

            List<ProductsReportViewModel> model = new List<ProductsReportViewModel>();

            foreach (var item in groupJoin)
            {
                ProductsReportViewModel viewModel = new ProductsReportViewModel();
                viewModel.Name = item.name;
                viewModel.Sum = item.Amount;
                model.Add(viewModel);
            }

            return model;
        }

        public async Task<List<ProductsReportViewModel>> Top10ProductsBySalesAmount(CancellationToken cancellationToken = default)
        {
            var groupJoin = await (from T1 in _context.SalesOrderDetails
                                   join T2 in _context.Products on T1.ProductId equals T2.ProductId
                                   //orderby T1.LineTotal descending
                                   group T1 by new { T1.Product.Name } into g
                                   select new
                                   {
                                       name = g.Key.Name,

                                       Amount = g.Sum(t => t.LineTotal)
                                   }).OrderByDescending(x => x.Amount).Take(10).ToListAsync(cancellationToken);


            List<ProductsReportViewModel> model = new List<ProductsReportViewModel>();

            foreach (var item in groupJoin)
            {
                ProductsReportViewModel viewModel = new ProductsReportViewModel();
                viewModel.Name = item.name;
                viewModel.Sum = item.Amount;
                model.Add(viewModel);
            }

            return model;
        }
    }

}

