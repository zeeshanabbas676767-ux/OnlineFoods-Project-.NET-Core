using System.Diagnostics;
using NewCoreProject.Models;
using Microsoft.AspNetCore.Mvc;
using NewCoreProject.Data;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System;
using NewCoreProject.Helpers;
using Microsoft.EntityFrameworkCore;
using NewCoreProject.Repositories;

namespace NewCoreProject.Controllers
{
    public class HomeController : Controller
    {
      private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;

        public HomeController(
     ILogger<HomeController> logger,
     IGenericRepository<User> userRepo,
     IGenericRepository<Product> productRepo,
     IGenericRepository<Category> categoryRepo,
     IGenericRepository<Order> orderRepo,
     IWebHostEnvironment env)
        {
            _logger = logger;
            _userRepo = userRepo;
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _orderRepo = orderRepo;
            _env = env;
        }
        public IActionResult IndexCustomer() 
        {
            return View();
        }
        public IActionResult IndexAdmin() 
        {
            return View();
        }

        public IActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }
        public IActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
        // ---------- Sign Up ----------
        public IActionResult SignUp() => View();
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult SignUp(User user, string passwordConfirm)
        {
            if (_userRepo.GetAll().Any(u => u.Admin_Email == user.Admin_Email))
                ModelState.AddModelError("Admin_Email", "Email already registered.");

            if (user.Admin_Password != passwordConfirm)
                ModelState.AddModelError("Admin_Password", "Passwords do not match.");

            if (!ModelState.IsValid) return View(user);

            user.Admin_Password = Hash(user.Admin_Password ?? "");
            user.CreatedAt = DateTime.Now;
            user.IsActive = true;

            _userRepo.Add(user);
            _userRepo.Save();

            SaveSession(user);
            TempData["Success"] = $"Account created successfully! Welcome, {user.Admin_Name}";
            return RedirectToAction("Login");
        }

        // ---------- Login ----------
        public IActionResult Login() => View();
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Login(User u, string password)
        {
            var user = _userRepo.GetAll().FirstOrDefault(d => d.Admin_Email == u.Admin_Email);
            if (user == null || user.Admin_Password != Hash(password))
            {
                ViewBag.Error = "Invalid password or username";
                return View(u);
            }

            if (!user.IsActive)
            {
                ViewBag.Error = "Your account is not active, please contact support";
                return View(u);
            }

            SaveSession(user);
            TempData["Success"] = $"Welcome, {user.Admin_Email}";
            return RedirectToAction("IndexCustomer", "Home");
        }

        // ---------- Logout ----------
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        // ---------- Upload Profile ----------
        //[HttpPost]
        //public JsonResult UploadProfileImage(IFormFile file, int userId)
        //{
        //    userId = userId == 0 ? HttpContext.Session.GetInt32("Admin_Id") ?? 0 : userId;
        //    if (file is null || file.Length == 0) return Json(new { success = false });

        //    var user = _context.User.Find(userId);
        //    if (user is null) return Json(new { success = false });

        //    user.ProfileImagePath = SaveFile(file);
        //    _context.SaveChanges();

        //    HttpContext.Session.SetString("ProfileImage", user.ProfileImagePath);
        //    return Json(new { success = true, imageUrl = user.ProfileImagePath });
        //}
        private static string Hash(string input)
        {
            using var sha = SHA256.Create();
            return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(input)))
                   .Replace("-", "").ToLower();
        }

        // ---------- Helpers ----------
        private void SaveSession(User u)
        {
            HttpContext.Session.SetInt32("Admin_Id", u.Admin_Id);
            HttpContext.Session.SetString("Admin_Name", u.Admin_Name ?? "");
            HttpContext.Session.SetString("Admin_Email", u.Admin_Email ?? "");
            HttpContext.Session.SetString("Admin_Address", u.Admin_Address ?? "");
            HttpContext.Session.SetString("ProfileImage",
                string.IsNullOrEmpty(u.ProfileImagePath) ? "/images/admin.png" : u.ProfileImagePath);
        }
        //private string SaveFile(IFormFile file)
        //{
        //    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        //    string dir = Path.Combine(_env.WebRootPath, "images");
        //    Directory.CreateDirectory(dir);

        //    string fullPath = Path.Combine(dir, fileName);
        //    using var stream = new FileStream(fullPath, FileMode.Create);
        //    file.CopyTo(stream);

        //    return "/images/" + fileName;
        //}
        //public IActionResult Login()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public IActionResult Login(User u)
        //{
        //    var Result = _context.Admins.Where(a => a.Admin_Email == u.Admin_Email && a.Admin_Password == u.Admin_Password).Count();
        //    if (Result == 1)
        //    {
        //        return RedirectToAction("Index", "Home");

        //    }
        //    else
        //    {
        //        ViewBag.Message = "Try Again";
        //        return View();
        //    }
        //}
        public IActionResult Cart()
        {
            return View();
        }
        public IActionResult DisplayProduct(int? id)
        {
            Shop s = new Shop();
            s.Cate = _categoryRepo.GetAll().ToList();

            if (id == null)
                s.Pro = _productRepo.GetAll().ToList();
            else
                s.Pro = _productRepo.GetAll().Where(m => m.Category_Fid == id).ToList();

            return View(s);
        }

        public IActionResult Feedback()
        {
            return View();
        }
        // ✅ Add To Cart
        public IActionResult AddToCart(int id)
        {
            var list = HttpContext.Session.GetObject<List<Product>>("mycart") ?? new List<Product>();

            var product = _productRepo.GetAll().FirstOrDefault(p => p.Product_Id == id);
            if (product != null)
            {
                product.Prod_Quantity = 1;
                list.Add(product);
            }

            HttpContext.Session.SetObject("mycart", list);
            return RedirectToAction("Cart", "Home");
        }

        // ✅ Decrease Quantity
        public IActionResult MinusFromCart(int rowNo)
        {
            var list = HttpContext.Session.GetObject<List<Product>>("mycart") ?? new List<Product>();

            if (rowNo >= 0 && rowNo < list.Count)
            {
                if (list[rowNo].Prod_Quantity > 1)
                    list[rowNo].Prod_Quantity--;
            }

            HttpContext.Session.SetObject("mycart", list);
            return RedirectToAction("Cart", "Home");
        }

        // ✅ Increase Quantity
        public IActionResult PlusFromCart(int rowNo)
        {
            var list = HttpContext.Session.GetObject<List<Product>>("mycart") ?? new List<Product>();

            if (rowNo >= 0 && rowNo < list.Count)
            {
                list[rowNo].Prod_Quantity++;
            }

            HttpContext.Session.SetObject("mycart", list);
            return RedirectToAction("Cart", "Home");
        }

        // ✅ Remove From Cart
        public IActionResult RemoveFromCart(int rowNo)
        {
            var list = HttpContext.Session.GetObject<List<Product>>("mycart") ?? new List<Product>();

            if (rowNo >= 0 && rowNo < list.Count)
            {
                list.RemoveAt(rowNo);
            }

            HttpContext.Session.SetObject("mycart", list);
            return RedirectToAction("Cart", "Home");
        }

        // ✅ PayPal Redirect
        public IActionResult PlayNow(Order o)
        {
            o.Order_Date = DateTime.Now;
            o.Order_Status = "Paid";
            o.Order_Type = "Sale";

            HttpContext.Session.SetObject("Order", o);

            double totalAmount = HttpContext.Session.GetObject<double>("totalAmount");
            double convertedAmount = totalAmount / 282; // Example conversion rate
            return Redirect($"https://sandbox.paypal.com/cgi-bin/webscr?cmd=_xclick&business=sb-lj439s41015447@personal.example.com&item_name=TheWayShopProducts&amount={convertedAmount}&currency_code=USD&return=https://localhost:44369/Home/OrderBooked&cancel_return=https://localhost:44369/Home/CancelOrder");
        }

        // ✅ Order Confirmation
        public IActionResult OrderBooked()
        {
            var order = HttpContext.Session.GetObject<Order>("Order");
            return View(order);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
