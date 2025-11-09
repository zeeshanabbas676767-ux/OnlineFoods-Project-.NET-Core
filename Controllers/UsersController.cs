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
    public class UsersController : Controller
    {
        private readonly IGenericRepository<User> _context;

        public UsersController(IGenericRepository<User> context)
        {
            _context = context;
        }

        // GET: Users
        public IActionResult Index()
        {
            var user = _context.GetAll();
            return View(user);
        }

        // GET: Users/Details/5
        public IActionResult Details(int id)
        {
            if (id == 0) return NotFound();
            var user = _context.GetById(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                _context.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public IActionResult Edit(int id)
        {
            var user = _context.GetById(id);
            if (user == null)  return NotFound();
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                    _context.Update(user);
                    _context.Save();
                
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public IActionResult Delete(int id)
        {
            var user = _context.GetById(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
             _context.Delete(id);
            _context.Save();
            return RedirectToAction(nameof(Index));
        }

        //private bool UserExists(int id)
        //{
        //    return _context.Admins.Any(e => e.Admin_Id == id);
        //}
    }
}
