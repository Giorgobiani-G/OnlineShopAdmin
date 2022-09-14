using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using OnlineShopAdmin.DataAccess.DbContexts;
using OnlineShopAdmin.DataAccess.Models;
using OnlineShopAdmin.DataAccess.Repository;
using OnlineShopAdmin.Filters;

namespace OnlineShopAdmin.Controllers
{
    [HttpRequestInfo]
    public class CustomersController : Controller
    {
        private readonly AdventureWorksLT2019Context _context;
        private readonly IRepository<Customer> _customerRepository;

        public CustomersController(AdventureWorksLT2019Context context, IRepository<Customer> customerRepository)
        {
            _context = context;
            _customerRepository = customerRepository;
        }

        // GET: Customers
        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {
            return View(await _customerRepository.GetListAsync(cancellationToken));
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int id,CancellationToken cancellationToken = default)
        {
            var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);

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
        public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken = default)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _customerRepository.GetByIdAsync((int)id, cancellationToken);
            if (customer == null)
            {
                return NotFound();
            }
            TempData["pa"] = customer.PasswordHash;
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Customer customer, CancellationToken cancellationToken = default)
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
                    if (!await _customerRepository.CustomerExists(customer.CustomerId))
                    {
                        
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
                
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken = default)
        {
            await _customerRepository.DeleteAsynch(id, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}
