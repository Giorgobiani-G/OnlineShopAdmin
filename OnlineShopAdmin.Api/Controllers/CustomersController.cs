using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopAdmin.DataAccess.Models;
using OnlineShopAdmin.DataAccess.Repository;
using OnlineShopAdmin.Filters;
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
            TempData["page"] = pg;
            ViewData["CurrentFilter"] = search;
            return View(list);
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int id, int pg, CancellationToken cancellationToken = default)
        {
            TempData["page"] = pg;

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
        public async Task<IActionResult> Edit(int? id, int pg, CancellationToken cancellationToken = default)
        {
            TempData["page"] = pg;

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
        public async Task<IActionResult> Edit(int id, int pg, Customer customer, CancellationToken cancellationToken = default)
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
                return RedirectToAction(nameof(Index), new { pg });
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int id, int pg, CancellationToken cancellationToken = default)
        {
            TempData["page"] = pg;

            var customer = await _customerRepository.GetByIdAsync(id, cancellationToken: cancellationToken);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, int pg, CancellationToken cancellationToken = default)
        {
            await _customerRepository.DeleteAsynch(id, cancellationToken);
            return RedirectToAction(nameof(Index), new { pg });
        }
    }
}
