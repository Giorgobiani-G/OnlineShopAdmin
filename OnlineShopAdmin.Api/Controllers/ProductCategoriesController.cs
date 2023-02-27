using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShopAdmin.Common.Repository;
using OnlineShopAdmin.DataAccess.Models;
using OnlineShopAdmin.Filters;
using System.Collections.Generic;
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
        public async Task<IActionResult> Index(int pg, int pageSize, string search, CancellationToken cancellationToken = default)
        {
            var (list, pagerDetails) = await _productCategoryRepository.GetListAsync(pg, pageSize, search, new string[] { "ParentProductCategory", "Products" }, cancellationToken);
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

        // GET: ProductCategories/Details/5
        public async Task<IActionResult> Details(int? id, int pg, int pageSize, string search, CancellationToken cancellationToken = default)
        {
            TempData["page"] = pg;
            TempData["currentFilter"] = search;
            TempData["pageSize"] = pageSize;

            if (id == null)
            {
                return NotFound();
            }
            var productCategory = await _productCategoryRepository.GetByIdAsync((int)id, new string[] { "ParentProductCategory" }, cancellationToken);

            if (productCategory == null)
            {
                return NotFound();
            }

            return View(productCategory);
        }

        // GET: ProductCategories/Create
        public IActionResult Create()
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
                await _productCategoryRepository.InsertAsync(productCategory, cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentProductCategoryId"] = new SelectList(_productCategoryRepository.GetList(), "ProductCategoryId", "Name", productCategory.ParentProductCategoryId);
            return View(productCategory);
        }

        // GET: ProductCategories/Edit/5
        public async Task<IActionResult> Edit(int? id, int pg, int pageSize, string search, CancellationToken cancellationToken = default)
        {
            TempData["page"] = pg;
            TempData["currentFilter"] = search;
            TempData["pageSize"] = pageSize;

            if (id == null)
            {
                return NotFound();
            }

            var productCategory = await _productCategoryRepository.GetByIdAsync((int)id, cancellationToken: cancellationToken);
            if (productCategory == null)
            {
                return NotFound();
            }
            ViewData["ParentProductCategoryId"] = new SelectList(_productCategoryRepository.GetList(), "ProductCategoryId", "Name", productCategory.ParentProductCategoryId);
            return View(productCategory);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductCategory productCategory, int pg, int pageSize, string search, CancellationToken cancellationToken = default)
        {
            if (id != productCategory.ProductCategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _productCategoryRepository.UpdateAsync(productCategory, cancellationToken);
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
                return RedirectToAction(nameof(Index), new { pg, pageSize, search });
            }
            ViewData["ParentProductCategoryId"] = new SelectList(_productCategoryRepository.GetList(), "ProductCategoryId", "Name", productCategory.ParentProductCategoryId);
            return View(productCategory);
        }

        // GET: ProductCategories/Delete/5
        public async Task<IActionResult> Delete(int? id, int pg, int pageSize, string search, CancellationToken cancellationToken = default)
        {
            TempData["page"] = pg;
            TempData["currentFilter"] = search;
            TempData["pageSize"] = pageSize;

            if (id == null)
            {
                return NotFound();
            }

            var productCategory = await _productCategoryRepository.GetByIdAsync((int)id, new string[] { "ParentProductCategory" }, cancellationToken);

            if (productCategory == null)
            {
                return NotFound();
            }

            return View(productCategory);
        }

        // POST: ProductCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int pg, int pageSize, string search, CancellationToken cancellationToken = default)
        {
            await _productCategoryRepository.DeleteAsync(id, cancellationToken);
            return RedirectToAction(nameof(Index), new { pg, pageSize, search });
        }
    }
}
