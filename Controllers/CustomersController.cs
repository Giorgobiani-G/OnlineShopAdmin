using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShopAdmin.DataAccess.DbContexts;
using OnlineShopAdmin.DataAccess.Models;
using OnlineShopAdmin.Filters;

namespace OnlineShopAdmin.Controllers
{
    [CustomFilter]
    public class CustomersController : Controller
    {
        private readonly AdventureWorksLT2019Context _context;
       
        public CustomersController(AdventureWorksLT2019Context context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            throw new NullReferenceException();
            return View(await _context.Customers.ToListAsync());
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
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

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,NameStyle,Title,FirstName,MiddleName,LastName,Suffix,CompanyName,SalesPerson,EmailAddress,Phone,PasswordHash,PasswordSalt,Rowguid,ModifiedDate")] Customer customer)
        {
           


            if (ModelState.IsValid)
            {
                //HashAlgorithm algorithm = new SHA256Managed();

                //byte[] plainTextWithSaltBytes =
                //  new byte[customer.PasswordHash.Length + customer.PasswordSalt.Length];

                //for (int i = 0; i < customer.PasswordHash.Length; i++)
                //{
                //    plainTextWithSaltBytes[i] = (byte)customer.PasswordHash[i];
                //}
                //for (int i = 0; i < customer.PasswordSalt.Length; i++)
                //{
                //    plainTextWithSaltBytes[customer.PasswordHash.Length + i] = (byte)customer.PasswordSalt[i];
                //}

                // var a = algorithm.ComputeHash(plainTextWithSaltBytes);

                //byte[] aa =
                //  new byte[customer.PasswordSalt.Length];
                //var b = algorithm.ComputeHash(a);
                //int nSalt = Convert.ToByte(customer.PasswordSalt.Length);

                //RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                //byte[] buff = new byte[nSalt];
                //rng.GetBytes(buff);
                //customer.PasswordSalt = Convert.ToBase64String(buff);

                //byte[] bytes = Encoding.UTF8.GetBytes(customer.PasswordSalt + customer.PasswordSalt);
                //SHA256Managed sHA256ManagedString = new SHA256Managed();
                //byte[] hash = sHA256ManagedString.ComputeHash(bytes);
                //customer.PasswordHash = Convert.ToBase64String(hash);

                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            TempData["pa"] = customer.PasswordHash;
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,NameStyle,Title,FirstName,MiddleName,LastName,Suffix,CompanyName,SalesPerson,EmailAddress,Phone,PasswordHash,PasswordSalt,Rowguid,ModifiedDate")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //string passwordchanged = (string)TempData["pa"];
                    //bool result = string.Equals(passwordchanged, customer.PasswordHash);
                    
                    //if (result==false)
                    //{
                    //    int nSalt = Convert.ToByte(customer.PasswordSalt.Length);

                    //    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    //    byte[] buff = new byte[nSalt];
                    //    rng.GetBytes(buff);
                    //    customer.PasswordSalt = Convert.ToBase64String(buff);

                    //    byte[] bytes = Encoding.UTF8.GetBytes(customer.PasswordSalt + customer.PasswordSalt);
                    //    SHA256Managed sHA256ManagedString = new SHA256Managed();
                    //    byte[] hash = sHA256ManagedString.ComputeHash(bytes);
                    //    customer.PasswordHash = Convert.ToBase64String(hash);
                    //}
                   

                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
