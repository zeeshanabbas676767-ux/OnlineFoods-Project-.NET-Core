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
    public class CategoriesController : Controller
    {
        private readonly IGenericRepository<Category> _context;

        public CategoriesController(IGenericRepository<Category> context)
        {
            _context = context;
        }

        // GET: Categories
        public IActionResult Index()
        {
            var category = _context.GetAll();
            return View(category);
        }

        // GET: Categories/Details/5
        public IActionResult Details(int id)
        {
          var categories = _context.GetById(id);
            if (categories == null) return NotFound();
            return View(categories);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                _context.Save(); 
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public IActionResult Edit(int id)
        {
            var category = _context.GetById(id);
            if(category == null) return NotFound();
            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
          if(ModelState.IsValid)
            {
                _context.Update(category);
                _context.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public IActionResult Delete(int id)
        {
            var category = _context.GetById(id);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
          _context.Delete(id);
            _context.Save();
            return RedirectToAction(nameof(Index));
        }

        //private bool CategoryExists(int id)
        //{
        //    return _context.Categories.Any(e => e.Category_Id == id);
        //}
    }
}
