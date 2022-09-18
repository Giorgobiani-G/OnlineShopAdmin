using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopAdmin.DataAccess.DbContexts;
using OnlineShopAdmin.DataAccess.Models;
using OnlineShopAdmin.DataAccess.Repository;
using OnlineShopAdmin.Filters;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopAdmin.Controllers
{
    [HttpRequestInfo]
    public class AddressesController : Controller
    {
        private readonly IRepository<Address> _addressRepository;

        public AddressesController(IRepository<Address> addressRepository)
        {
            _addressRepository = addressRepository;
        }

        // GET: Addresses
        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {
            return View(await _addressRepository.GetListAsync(cancellationToken));
        }

        // GET: Addresses/Details/5
        public async Task<IActionResult> Details(int? id, CancellationToken cancellationToken = default)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _addressRepository.GetByIdAsync((int)id, cancellationToken);
            if (address == null)
            {
                return NotFound();
            }

            return View(address);
        }

        // GET: Addresses/Create
        public IActionResult Create()
        {
            return View();
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Address address, CancellationToken cancellationToken = default)
        {
            if (ModelState.IsValid)
            {
                await _addressRepository.InseretAsynch(address, cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            return View(address);
        }

        // GET: Addresses/Edit/5
        public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken = default)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _addressRepository.GetByIdAsync((int)id, cancellationToken);
            if (address == null)
            {
                return NotFound();
            }
            return View(address);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Address address, CancellationToken cancellationToken = default)
        {
            if (id != address.AddressId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _addressRepository.UpdateAsynch(address, cancellationToken);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _addressRepository.CustomExists(address.AddressId))
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
            return View(address);
        }

        // GET: Addresses/Delete/5
        public async Task<IActionResult> Delete(int? id, CancellationToken cancellationToken = default)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _addressRepository.GetByIdAsync((int)id, cancellationToken);
            if (address == null)
            {
                return NotFound();
            }

            return View(address);
        }

        // POST: Addresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken = default)
        {
            await _addressRepository.DeleteAsynch(id, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}
