﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShopAdmin.DataAccess.DbContexts;
using OnlineShopAdmin.DataAccess.Models;
using OnlineShopAdmin.DataAccess.Repository;
using OnlineShopAdmin.Filters;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopAdmin.Controllers
{
    [HttpRequestInfo]
    public class SalesOrderHeadersController : Controller
    {
        private readonly IRepository<SalesOrderHeader> _salesOrderHeaderRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Address> _addressRepository;

        public SalesOrderHeadersController(IRepository<SalesOrderHeader> salesOrderHeaderRepository,
                                           IRepository<Customer> customerRepository,
                                           IRepository<Address> addressRepository)
        {
            _salesOrderHeaderRepository = salesOrderHeaderRepository;
            _customerRepository = customerRepository;
            _addressRepository = addressRepository;
        }

        // GET: SalesOrderHeaders
        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {
            var adventureWorksLT2019Context = await _salesOrderHeaderRepository.GetListAsync(cancellationToken, new string[] { "BillToAddress", "Customer", "ShipToAddress" });
            return View(adventureWorksLT2019Context);
        }

        // GET: SalesOrderHeaders/Details/5
        public async Task<IActionResult> Details(int? id, CancellationToken cancellationToken = default)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrderHeader = await _salesOrderHeaderRepository.GetByIdAsync((int)id, cancellationToken, new string[] { "BillToAddress", "Customer", "ShipToAddress" });

            if (salesOrderHeader == null)
            {
                return NotFound();
            }

            return View(salesOrderHeader);
        }

        // GET: SalesOrderHeaders/Create
        public IActionResult Create()
        {
            ViewData["BillToAddressId"] = new SelectList(_addressRepository.GetList(), "AddressId", "AddressLine1");
            ViewData["CustomerId"] = new SelectList(_customerRepository.GetList(), "CustomerId", "FirstName");
            ViewData["ShipToAddressId"] = new SelectList(_addressRepository.GetList(), "AddressId", "AddressLine1");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SalesOrderHeader salesOrderHeader, CancellationToken cancellationToken = default)
        {
            if (ModelState.IsValid)
            {
                await _salesOrderHeaderRepository.InseretAsynch(salesOrderHeader, cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            ViewData["BillToAddressId"] = new SelectList(_addressRepository.GetList(), "AddressId", "AddressLine1", salesOrderHeader.BillToAddressId);
            ViewData["CustomerId"] = new SelectList(_customerRepository.GetList(), "CustomerId", "FirstName", salesOrderHeader.CustomerId);
            ViewData["ShipToAddressId"] = new SelectList(_addressRepository.GetList(), "AddressId", "AddressLine1", salesOrderHeader.ShipToAddressId);
            return View(salesOrderHeader);
        }

        // GET: SalesOrderHeaders/Edit/5
        public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken = default)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrderHeader = await _salesOrderHeaderRepository.GetByIdAsync((int)id, cancellationToken);
            if (salesOrderHeader == null)
            {
                return NotFound();
            }
            ViewData["BillToAddressId"] = new SelectList(_addressRepository.GetList(), "AddressId", "AddressLine1", salesOrderHeader.BillToAddressId);
            ViewData["CustomerId"] = new SelectList(_customerRepository.GetList(), "CustomerId", "FirstName", salesOrderHeader.CustomerId);
            ViewData["ShipToAddressId"] = new SelectList(_addressRepository.GetList(), "AddressId", "AddressLine1", salesOrderHeader.ShipToAddressId);
            return View(salesOrderHeader);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SalesOrderHeader salesOrderHeader, CancellationToken cancellationToken = default)
        {
            if (id != salesOrderHeader.SalesOrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _salesOrderHeaderRepository.UpdateAsynch(salesOrderHeader, cancellationToken);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _salesOrderHeaderRepository.CustomExists(salesOrderHeader.SalesOrderId))
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
            ViewData["BillToAddressId"] = new SelectList(_addressRepository.GetList(), "AddressId", "AddressLine1", salesOrderHeader.BillToAddressId);
            ViewData["CustomerId"] = new SelectList(_customerRepository.GetList(), "CustomerId", "FirstName", salesOrderHeader.CustomerId);
            ViewData["ShipToAddressId"] = new SelectList(_addressRepository.GetList(), "AddressId", "AddressLine1", salesOrderHeader.ShipToAddressId);
            return View(salesOrderHeader);
        }

        // GET: SalesOrderHeaders/Delete/5
        public async Task<IActionResult> Delete(int? id, CancellationToken cancellationToken = default)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrderHeader = await _salesOrderHeaderRepository.GetByIdAsync((int)id, cancellationToken, new string[] { "BillToAddress", "Customer", "ShipToAddress" });

            if (salesOrderHeader == null)
            {
                return NotFound();
            }

            return View(salesOrderHeader);
        }

        // POST: SalesOrderHeaders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken = default)
        {
            var salesOrderHeader = await _salesOrderHeaderRepository.GetByIdAsync(id, cancellationToken);
            await _salesOrderHeaderRepository.DeleteAsynch(salesOrderHeader, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}
