using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewCoreProject.Data;
using NewCoreProject.Models;
using NewCoreProject.Repositories;

namespace NewCoreProject.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IWebHostEnvironment _env;
        public ProductsController(
            IGenericRepository<Product> productRepo,
            IGenericRepository<Category> categoryRepo,
            IWebHostEnvironment env)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _env = env;
        }

        // GET: Products
        public IActionResult Index()
        {
            // We can’t Include() through generic repository directly,
            // so for now just get all products (simple version)
            var products = _productRepo.GetAll();
            return View(products);
        }

        // GET: Products/Details/5
        public IActionResult Details(int id)
        {
            var product = _productRepo.GetById(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["Category_Fid"] = new SelectList(_categoryRepo.GetAll(), "Category_Id", "Category_Name");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IFormFile? Pro_Pic, Product product)
        {
            if (ModelState.IsValid)
            {
                if (Pro_Pic != null)
                {
                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(Pro_Pic.FileName)}";
                    string path = Path.Combine(_env.WebRootPath, "images", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        Pro_Pic.CopyTo(stream);
                    }
                    product.Product_Picture = "/images/" + fileName;
                }

                _productRepo.Add(product);
                _productRepo.Save();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Category_Fid"] = new SelectList(_categoryRepo.GetAll(), "Category_Id", "Category_Name", product.Category_Fid);
            return View(product);
        }

        // GET: Products/Edit/5
        public IActionResult Edit(int id)
        {
            var product = _productRepo.GetById(id);
            if (product == null)
                return NotFound();

            ViewData["Category_Fid"] = new SelectList(_categoryRepo.GetAll(), "Category_Id", "Category_Name", product.Category_Fid);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Product product, IFormFile? Pro_Pic)
        {
            if (id != product.Product_Id) return NotFound();

            var existing = _productRepo.GetById(id);
            if (existing == null) return NotFound();

            if (ModelState.IsValid)
            {
                existing.Product_Name = product.Product_Name;
                existing.Product_PurchasePrice = product.Product_PurchasePrice;
                existing.Product_SalePrice = product.Product_SalePrice;
                existing.Category_Fid = product.Category_Fid;
                existing.Product_Description = product.Product_Description;
                existing.Pro_Pic = product.Pro_Pic;
                // Handle new image
                if (Pro_Pic != null)
                {
                    if (!string.IsNullOrEmpty(existing.Product_Picture))
                    {
                        var oldPath = Path.Combine(_env.WebRootPath, existing.Product_Picture.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(Pro_Pic.FileName)}";
                    string path = Path.Combine(_env.WebRootPath, "images", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        Pro_Pic.CopyTo(stream);
                    }

                    //product.Product_Picture = "/images/" + fileName;
                    existing.Product_Picture = "/images/" + fileName;
                }

                _productRepo.Update(existing);
                _productRepo.Save();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Category_Fid"] = new SelectList(_categoryRepo.GetAll(), "Category_Id", "Category_Name", product.Category_Fid);
            return View(product);
        }

        // GET: Products/Delete/5
        public IActionResult Delete(int id)
        {
            var product = _productRepo.GetById(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _productRepo.GetById(id);
            if (product != null)
            {
                // delete image from server
                if (!string.IsNullOrEmpty(product.Product_Picture))
                {
                    var path = Path.Combine(_env.WebRootPath, product.Product_Picture.TrimStart('/'));
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }

                _productRepo.Delete(id);
                _productRepo.Save();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
