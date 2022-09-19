using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShopAdmin.DataAccess.DbContexts;
using OnlineShopAdmin.DataAccess.Models;
using OnlineShopAdmin.DataAccess.Repository;
using OnlineShopAdmin.Filters;

namespace OnlineShopAdmin.Controllers
{
    [HttpRequestInfo]
    public class ProductsController : Controller
    {
        private readonly IRepository<Product> _productsRepository;
        private readonly IRepository<ProductCategory> _productsCategoryRepository;
        private readonly IRepository<ProductModel> _productsModelRepository;

        public ProductsController(IRepository<Product> productsRepository,
                                  IRepository<ProductCategory> productsCategoryRepository,
                                  IRepository<ProductModel> productsModelRepository)
        {
            _productsRepository = productsRepository;
            _productsCategoryRepository = productsCategoryRepository;
            _productsModelRepository = productsModelRepository;
        }

        // GET: Products
        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {
            var adventureWorksLT2019Context = await _productsRepository.GetListAsync(cancellationToken, new string[] { "ProductCategory", "ProductModel", "SalesOrderDetails" });
            return View(adventureWorksLT2019Context);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id, CancellationToken cancellationToken = default)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productsRepository.GetByIdAsync((int)id, cancellationToken, new string[] { "ProductCategory", "ProductModel" });

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["ProductCategoryId"] = new SelectList(_productsCategoryRepository.GetList(), "ProductCategoryId", "Name");
            ViewData["ProductModelId"] = new SelectList(_productsModelRepository.GetList(), "ProductModelId", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, CancellationToken cancellationToken = default)
        {
            if (ModelState.IsValid)
            {
                if (product.Photo != null)
                {
                    string UniequeFileName = Guid.NewGuid().ToString().Substring(0, 10) + "_" + product.Photo.FileName;

                    var path = Path.Combine(
                                Directory.GetCurrentDirectory(), "wwwroot", "Images",
                                UniequeFileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await product.Photo.CopyToAsync(stream);
                    }
                    product.ThumbnailPhotoFileName = UniequeFileName;
                }
                await _productsRepository.InseretAsynch(product, cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductCategoryId"] = new SelectList(_productsCategoryRepository.GetList(), "ProductCategoryId", "Name", product.ProductCategoryId);
            ViewData["ProductModelId"] = new SelectList(_productsModelRepository.GetList(), "ProductModelId", "Name", product.ProductModelId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken = default)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productsRepository.GetByIdAsync((int)id, cancellationToken);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["ProductCategoryId"] = new SelectList(_productsCategoryRepository.GetList(), "ProductCategoryId", "Name", product.ProductCategoryId);
            ViewData["ProductModelId"] = new SelectList(_productsModelRepository.GetList(), "ProductModelId", "Name", product.ProductModelId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Product product, CancellationToken cancellationToken = default)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (product.Photo != null)
                    {
                        if (product.ThumbnailPhotoFileName is not null)
                        {
                            FileInfo file = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", product.ThumbnailPhotoFileName));
                            file.Delete();
                        }
                        string UniequeFileName = Guid.NewGuid().ToString().Substring(0, 10) + "_" + product.Photo.FileName;

                        var path = Path.Combine(
                                    Directory.GetCurrentDirectory(), "wwwroot", "Images",
                                    UniequeFileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await product.Photo.CopyToAsync(stream);
                        }
                        product.ThumbnailPhotoFileName = UniequeFileName;
                    }

                    await _productsRepository.UpdateAsynch(product, cancellationToken);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _productsRepository.CustomExists(product.ProductId))
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
            ViewData["ProductCategoryId"] = new SelectList(_productsCategoryRepository.GetList(), "ProductCategoryId", "Name", product.ProductCategoryId);
            ViewData["ProductModelId"] = new SelectList(_productsModelRepository.GetList(), "ProductModelId", "Name", product.ProductModelId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id, CancellationToken cancellationToken = default)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productsRepository.GetByIdAsync((int)id, cancellationToken, new string[] { "ProductCategory", "ProductModel" });

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken = default)
        {
            //unit of work is needed here
            var product = await _productsRepository.GetByIdAsync(id,cancellationToken);
            if (product.ThumbnailPhotoFileName is not null)
            {
                FileInfo file = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", product.ThumbnailPhotoFileName));
                file.Delete();
            }

            await _productsRepository.DeleteAsynch(product, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}
