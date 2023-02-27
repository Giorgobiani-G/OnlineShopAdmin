using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using OnlineShopAdmin.Filters;
using OnlineShopAdmin.DataAccess;
using System.Threading;

namespace OnlineShopAdmin.Controllers
{
    [HttpRequestInfo]
    public class ReportsController : Controller
    {
        private readonly ReportingDA _reportingDA;
        public ReportsController(ReportingDA reportingDA)
        {
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
