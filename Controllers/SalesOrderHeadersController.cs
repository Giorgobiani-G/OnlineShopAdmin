﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShopAdmin.DataAccess.DbContexts;
using OnlineShopAdmin.DataAccess.Models;

namespace OnlineShopAdmin.Controllers
{
    public class SalesOrderHeadersController : Controller
    {
        private readonly AdventureWorksLT2019Context _context;

        public SalesOrderHeadersController(AdventureWorksLT2019Context context)
        {
            _context = context;
        }

        // GET: SalesOrderHeaders
        public async Task<IActionResult> Index()
        {
            var adventureWorksLT2019Context = _context.SalesOrderHeaders.Include(s => s.BillToAddress).Include(s => s.Customer).Include(s => s.ShipToAddress);
            return View(await adventureWorksLT2019Context.ToListAsync());
        }

        // GET: SalesOrderHeaders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrderHeader = await _context.SalesOrderHeaders
                .Include(s => s.BillToAddress)
                .Include(s => s.Customer)
                .Include(s => s.ShipToAddress)
                .FirstOrDefaultAsync(m => m.SalesOrderId == id);
            if (salesOrderHeader == null)
            {
                return NotFound();
            }

            return View(salesOrderHeader);
        }

        // GET: SalesOrderHeaders/Create
        public IActionResult Create()
        {
            ViewData["BillToAddressId"] = new SelectList(_context.Addresses, "AddressId", "AddressLine1");
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName");
            ViewData["ShipToAddressId"] = new SelectList(_context.Addresses, "AddressId", "AddressLine1");
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name");
            return View();
        }

        // POST: SalesOrderHeaders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SalesOrderId,RevisionNumber,OrderDate,DueDate,ShipDate,Status,OnlineOrderFlag,SalesOrderNumber,PurchaseOrderNumber,AccountNumber,CustomerId,ShipToAddressId,BillToAddressId,ShipMethod,CreditCardApprovalCode,SubTotal,TaxAmt,Freight,TotalDue,Comment,Rowguid,ModifiedDate,ProductId")] SalesOrderHeader salesOrderHeader)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salesOrderHeader);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BillToAddressId"] = new SelectList(_context.Addresses, "AddressId", "AddressLine1", salesOrderHeader.BillToAddressId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName", salesOrderHeader.CustomerId);
            ViewData["ShipToAddressId"] = new SelectList(_context.Addresses, "AddressId", "AddressLine1", salesOrderHeader.ShipToAddressId);
           
            return View(salesOrderHeader);
        }

        // GET: SalesOrderHeaders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrderHeader = await _context.SalesOrderHeaders.FindAsync(id);
            if (salesOrderHeader == null)
            {
                return NotFound();
            }
            ViewData["BillToAddressId"] = new SelectList(_context.Addresses, "AddressId", "AddressLine1", salesOrderHeader.BillToAddressId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName", salesOrderHeader.CustomerId);
            ViewData["ShipToAddressId"] = new SelectList(_context.Addresses, "AddressId", "AddressLine1", salesOrderHeader.ShipToAddressId);
            return View(salesOrderHeader);
        }

        // POST: SalesOrderHeaders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SalesOrderId,RevisionNumber,OrderDate,DueDate,ShipDate,Status,OnlineOrderFlag,SalesOrderNumber,PurchaseOrderNumber,AccountNumber,CustomerId,ShipToAddressId,BillToAddressId,ShipMethod,CreditCardApprovalCode,SubTotal,TaxAmt,Freight,TotalDue,Comment,Rowguid,ModifiedDate")] SalesOrderHeader salesOrderHeader)
        {
            if (id != salesOrderHeader.SalesOrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salesOrderHeader);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesOrderHeaderExists(salesOrderHeader.SalesOrderId))
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
            ViewData["BillToAddressId"] = new SelectList(_context.Addresses, "AddressId", "AddressLine1", salesOrderHeader.BillToAddressId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName", salesOrderHeader.CustomerId);
            ViewData["ShipToAddressId"] = new SelectList(_context.Addresses, "AddressId", "AddressLine1", salesOrderHeader.ShipToAddressId);
            return View(salesOrderHeader);
        }

        // GET: SalesOrderHeaders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrderHeader = await _context.SalesOrderHeaders
                .Include(s => s.BillToAddress)
                .Include(s => s.Customer)
                .Include(s => s.ShipToAddress)
                .FirstOrDefaultAsync(m => m.SalesOrderId == id);
            if (salesOrderHeader == null)
            {
                return NotFound();
            }

            return View(salesOrderHeader);
        }

        // POST: SalesOrderHeaders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salesOrderHeader = await _context.SalesOrderHeaders.FindAsync(id);
            _context.SalesOrderHeaders.Remove(salesOrderHeader);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalesOrderHeaderExists(int id)
        {
            return _context.SalesOrderHeaders.Any(e => e.SalesOrderId == id);
        }
    }
}
