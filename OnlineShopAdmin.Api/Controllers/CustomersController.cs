using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShopAdmin.DataAccess.Models;
using OnlineShopAdmin.DataAccess.Repository;
using OnlineShopAdmin.Filters;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopAdmin.Controllers
{
    [HttpRequestInfo]
    public class CustomersController : Controller
    {
        private readonly IRepository<Customer> _customerRepository;

        public CustomersController(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        // GET: Customers
        public async Task<IActionResult> Index(int pg, int pageSize, string search, CancellationToken cancellationToken = default)
        {
            var (list, pagerDetails) = await _customerRepository.GetListAsync(pg, pageSize, search, cancellationToken: cancellationToken);
            ViewBag.Pager = pagerDetails;
            ViewBag.PageSizes = GetPageSizes(pageSize);
            TempData["page"] = pg;
            TempData["currentFilter"] = search;
            TempData["pageSize"] = pageSize;
            return View(list);
        }

        private List<SelectListItem> GetPageSizes(int selectedPageSize)
        {
            var pageSizes = new List<SelectListItem>();

            for (int i = 20; i <= 50; i += 10)
            {
                if (i == selectedPageSize)
                {
                    pageSizes.Add(new SelectListItem(i.ToString(), i.ToString(), true));
                }
                else
                {
                    pageSizes.Add(new SelectListItem(i.ToString(), i.ToString()));
                }
            }

            return pageSizes;
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int id, int pg, int pageSize, string search, CancellationToken cancellationToken = default)
        {
            TempData["page"] = pg;
            TempData["currentFilter"] = search;
            TempData["pageSize"] = pageSize;

            var customer = await _customerRepository.GetByIdAsync(id, cancellationToken: cancellationToken);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer, CancellationToken cancellationToken = default)
        {
            if (ModelState.IsValid)
            {
                await _customerRepository.InseretAsynch(customer, cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id, int pg, int pageSize, string search, CancellationToken cancellationToken = default)
        {
            TempData["page"] = pg;
            TempData["currentFilter"] = search;
            TempData["pageSize"] = pageSize;

            if (id == null)
            {
                return NotFound();
            }

            var customer = await _customerRepository.GetByIdAsync((int)id, cancellationToken: cancellationToken);
            if (customer == null)
            {
                return NotFound();
            }
            TempData["pa"] = customer.PasswordHash;
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, int pg, int pageSize, string search, Customer customer, CancellationToken cancellationToken = default)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _customerRepository.UpdateAsynch(customer, cancellationToken);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _customerRepository.CustomExists(customer.CustomerId))
                    {

                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { pg, pageSize, search });
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int id, int pg, int pageSize, string search, CancellationToken cancellationToken = default)
        {
            TempData["page"] = pg;
            TempData["currentFilter"] = search;
            TempData["pageSize"] = pageSize;

            var customer = await _customerRepository.GetByIdAsync(id, cancellationToken: cancellationToken);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, int pg, int pageSize, string search, CancellationToken cancellationToken = default)
        {
            await _customerRepository.DeleteAsynch(id, cancellationToken);
            return RedirectToAction(nameof(Index), new { pg, pageSize, search });
        }
    }
}
