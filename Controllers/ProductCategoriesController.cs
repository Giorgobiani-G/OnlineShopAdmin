using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class ProductCategoriesController : Controller
    {
         
        private readonly IRepository<ProductCategory> _productCategoryRepository;


        public ProductCategoriesController(IRepository<ProductCategory> productCategoryRepository)
        {
            _productCategoryRepository = productCategoryRepository;
        }

        // GET: ProductCategories
        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {
            return View(await _productCategoryRepository.GetListAsync(cancellationToken, new string[] { "ParentProductCategory", "Products" }));
        }

        // GET: ProductCategories/Details/5
        public async Task<IActionResult> Details(int? id, CancellationToken cancellationToken = default)
        {
            if (id == null)
            {
                return NotFound();
            }
            var productCategory = await _productCategoryRepository.GetByIdAsync((int)id, cancellationToken, new string[] { "ParentProductCategory" });

            //var productCategory = await _context.ProductCategories
            //    .Include(p => p.ParentProductCategory)
            //    .FirstOrDefaultAsync(m => m.ProductCategoryId == id);
            if (productCategory == null)
            {
                return NotFound();
            }

            return View(productCategory);
        }

        // GET: ProductCategories/Create
        public IActionResult Create(CancellationToken cancellationToken = default)
        {
            ViewData["ParentProductCategoryId"] = new SelectList(_productCategoryRepository.GetList(), "ProductCategoryId", "Name");
            return View();
        }

        // POST: ProductCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCategory productCategory, CancellationToken cancellationToken = default)
        {
            if (ModelState.IsValid)
            {
                await _productCategoryRepository.InseretAsynch(productCategory, cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentProductCategoryId"] = new SelectList(_productCategoryRepository.GetList(), "ProductCategoryId", "Name", productCategory.ParentProductCategoryId);
            return View(productCategory);
        }

        // GET: ProductCategories/Edit/5
        public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken = default)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productCategory = await _productCategoryRepository.GetByIdAsync((int)id, cancellationToken);
            if (productCategory == null)
            {
                return NotFound();
            }
            ViewData["ParentProductCategoryId"] = new SelectList(_productCategoryRepository.GetList(), "ProductCategoryId", "Name", productCategory.ParentProductCategoryId);
            return View(productCategory);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductCategory productCategory, CancellationToken cancellationToken = default)
        {
            if (id != productCategory.ProductCategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _productCategoryRepository.UpdateAsynch(productCategory, cancellationToken);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _productCategoryRepository.CustomExists(productCategory.ProductCategoryId))
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
            ViewData["ParentProductCategoryId"] = new SelectList(_productCategoryRepository.GetList(), "ProductCategoryId", "Name", productCategory.ParentProductCategoryId);
            return View(productCategory);
        }

        // GET: ProductCategories/Delete/5
        public async Task<IActionResult> Delete(int? id, CancellationToken cancellationToken = default)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productCategory = await _productCategoryRepository.GetByIdAsync((int)id, cancellationToken, new string[] { "ParentProductCategory" });

            if (productCategory == null)
            {
                return NotFound();
            }

            return View(productCategory);
        }

        // POST: ProductCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken = default)
        {
            await _productCategoryRepository.DeleteAsynch(id, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}
