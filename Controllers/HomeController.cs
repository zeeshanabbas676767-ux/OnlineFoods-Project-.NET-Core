using System.Diagnostics;
using NewCoreProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;
using NewCoreProject.Helpers;
using NewCoreProject.Repositories;
using System.Collections.Generic;

namespace NewCoreProject.Controllers
{
    public class HomeController : Controller
    {
      private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<Shop> _shopRepo;
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly IGenericRepository<Order_Detail> _order_detail;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;

        public HomeController(
     ILogger<HomeController> logger,
     IGenericRepository<User> userRepo,
     IGenericRepository<Product> productRepo,
      IGenericRepository<Shop> shopRepo,
     IGenericRepository<Category> categoryRepo,
     IGenericRepository<Order> orderRepo,
       IGenericRepository<Order_Detail> order_details,
     IWebHostEnvironment env)
        {
            _logger = logger;
            _userRepo = userRepo;
            _productRepo = productRepo;
            _shopRepo = shopRepo;
            _categoryRepo = categoryRepo;
            _orderRepo = orderRepo;
            _order_detail = order_details; 

            _env = env;
        }
        public IActionResult IndexCustomer() 
        {
            return View();
        }
        //public IActionResult MainPage()
        //{
        //    return View();
        //}
        //public IActionResult HomeMiddleContent() 
        //{
        //    return PartialView("_HomeMiddleContent");
        //}
        //public IActionResult ProductContent()
        //{
        //    // Load categories + include products
        //    var categories = _categoryRepo.GetAll(c => c.Products).ToList();

        //    // Load all products
        //    var products = _productRepo.GetAll().ToList();

        //    // Build viewmodel
        //    var shop = new Shop
        //    {
        //        Cate = categories,
        //        Pro = products
        //    };

        //    return PartialView("_ProductContent", shop);
        //}
        //public IActionResult ProductContent()
        //{
        //    try
        //    {
        //        // ✅ Load categories safely
        //        var categories = _categoryRepo.GetAll() ?? new List<Category>();

        //        // For each category, ensure Products list is not null
        //        foreach (var c in categories)
        //        {
        //            c.Products = c.Products ?? new List<Product>();
        //        }

        //        // ✅ Load all products safely
        //        var products = _productRepo.GetAll() ?? new List<Product>();

        //        // ✅ Build viewmodel
        //        var shop = new Shop
        //        {
        //            Cate = (List<Category>)categories,
        //            Pro = (List<Product>)products
        //        };

        //        // Always pass the model to partial view
        //        return PartialView("_ProductContent", shop);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Optional: log the exception somewhere
        //        Console.WriteLine(ex.Message);

        //        // Return an empty view so AJAX still works
        //        var emptyShop = new Shop
        //        {
        //            Cate = new List<Category>(),
        //            Pro = new List<Product>()
        //        };
        //        return PartialView("_ProductContent", emptyShop);
        //    }
        //}

        // Load cart with optional product add
        //public IActionResult CartContent(int? id)
        //{
        //    var cart = HttpContext.Session.GetObject<List<Product>>("mycart") ?? new List<Product>();

        //    if (id.HasValue)
        //    {
        //        var product = _productRepo.GetAll().FirstOrDefault(p => p.Product_Id == id.Value);
        //        if (product != null)
        //        {
        //            var existing = cart.FirstOrDefault(p => p.Product_Id == id.Value);
        //            if (existing != null)
        //                existing.Prod_Quantity++;
        //            else
        //            {
        //                product.Prod_Quantity = 1;
        //                cart.Add(product);
        //            }
        //        }
        //        HttpContext.Session.SetObject("mycart", cart);
        //    }

        //    return PartialView("_CartContent", cart); // always pass cart as model
        //}

        public IActionResult IndexAdmin()
        {
            var totalUsers = _userRepo.GetAll().Count();
            var totalCategories = _categoryRepo.GetAll().Count();
            var totalProducts = _productRepo.GetAll().Count();

            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalCategories = totalCategories;
            ViewBag.TotalProducts = totalProducts;

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
            {
                ModelState.AddModelError("Admin_Email", "Email already registered.");
                TempData["OpenSignUp"] = true;  // reopen modal
                return View("~/Views/Shared/_SignUpPartial.cshtml", user);
            }
            if (user.Admin_Password != passwordConfirm)
                ModelState.AddModelError("Admin_Password", "Passwords do not match.");

            if (!ModelState.IsValid)
            {
                TempData["OpenSignUp"] = true;   // tell layout to reopen modal
                return View("~/Views/Shared/_SignUpPartial.cshtml", user); // small empty view
            }   

            user.Admin_Password = Hash(user.Admin_Password ?? "");
            user.CreatedAt = DateTime.Now;
            user.IsActive = true;

            _userRepo.Add(user);
            _userRepo.Save();

            SaveSession(user);
            TempData["Success"] = $"Account created successfully! Welcome, {user.Admin_Name}";
            TempData["ShowSignUpModal"] = "true";
            return RedirectToAction("IndexCustomer", "Home");
        }

        // ---------- Login ----------
        public IActionResult Login() => View();
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Login(User u, string password)
        {
            var user = _userRepo.GetAll().FirstOrDefault(d => d.Admin_Email == u.Admin_Email);

            if (user == null || user.Admin_Password != Hash(password))
            {
                TempData["OpenLogin"] = true;
                TempData["LoginError"] = "Invalid email or password";
                return View("~/Views/Shared/_LoginPartial.cshtml");
            }

            if (!user.IsActive)
            {
                TempData["OpenLogin"] = true;
                TempData["LoginError"] = "Your account is not active";
                return View("~/Views/Shared/_LoginPartial.cshtml");
            }

            // success
            SaveSession(user);
            TempData["Success"] = $"Welcome, {user.Admin_Email}";
            return RedirectToAction("IndexCustomer", "Home");
        }


        // ---------- Logout ----------
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("IndexCustomer", "Home");
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
        public IActionResult DisplayProduct(int? id, int page = 1, int pageSize = 12,
                                         int? minPrice = null, int? maxPrice = null)
        {
            Shop s = new Shop();
            s.Cate = _categoryRepo.GetAll().ToList();

            var products = _productRepo.GetAll().AsQueryable();

            // Filter by category
            if (id != null)
            {
                products = products.Where(p => p.Category_Fid == id);
            }

            // Filter by price range
            if (minPrice.HasValue)
                products = products.Where(p => p.Product_SalePrice >= minPrice.Value);

            if (maxPrice.HasValue)
                products = products.Where(p => p.Product_SalePrice <= maxPrice.Value);

            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            int totalProducts = products.Count();

            var pagedProducts = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            s.Pro = pagedProducts;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            ViewBag.CategoryId = id;

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
            // The cart must be retrieved and saved into the session for later use
            var cart = HttpContext.Session.GetObject<List<Product>>("mycart");

            if (cart == null || !cart.Any())
            {
                // Handle empty cart scenario, e.g., redirect with an error message
                TempData["Error"] = "Your cart is empty.";
                return PartialView("_CartContent", cart);
            }

            o.Order_Date = DateTime.Now;
            o.Order_Status = "Paid"; // Status will be confirmed after PayPal redirect
            o.Order_Type = "Sale";

            // Save Order and Cart into Session
            HttpContext.Session.SetObject("Order", o);
            HttpContext.Session.SetObject("OrderCart", cart); // Save the cart details

            double totalAmount = HttpContext.Session.GetObject<double>("totalAmount");
            double convertedAmount = totalAmount / 282; // Example conversion rate

            // The PayPal redirect URL should point back to OrderBooked
            return Redirect($"https://sandbox.paypal.com/cgi-bin/webscr?cmd=_xclick&business=sb-lj439s41015447@personal.example.com&item_name=TheWayShopProducts&amount={convertedAmount}&currency_code=USD&return=https://localhost:44369/Home/OrderBooked&cancel_return=https://localhost:44369/Home/CancelOrder");
        }
        //public IActionResult PlayNow(Order o, string paymentMethod = "PayPal")
        //{
        //    o.Order_Date = DateTime.Now;
        //    o.Order_Status = "Pending";
        //    o.Order_Type = "Sale";

        //    HttpContext.Session.SetObject("Order", o);

        //    double totalAmount = HttpContext.Session.GetObject<double>("totalAmount");

        //    // 🎯 Use the Factory Pattern here
        //    var payment = PaymentFactory.GetPaymentMethod(paymentMethod);
        //    string result = payment.Pay(totalAmount);

        //    // ✅ Redirect or handle based on payment type
        //    if (result == "CashOnDelivery")
        //    {
        //        o.Order_Status = "Cash on Delivery";
        //        _orderRepo.Add(o);
        //        _orderRepo.Save();
        //        return RedirectToAction("OrderBooked");
        //    }
        //    else
        //    {
        //        // PayPal or other redirect URL
        //        return Redirect(result);
        //    }
        //}

        // ✅ Order Confirmation
        public IActionResult OrderBooked()
        { 
            // 1. Retrieve the Order and Cart from the session
            var order = HttpContext.Session.GetObject<Order>("Order");
            var cart = HttpContext.Session.GetObject<List<Product>>("OrderCart");

            if (order == null || cart == null)
            {
                // Redirect if session data is missing (e.g., user navigated directly)
                TempData["Error"] = "Order data not found.";
                return RedirectToAction("IndexCustomer");
            }

            // 2. Save the Order and all its details to the database
            SaveOrderAndDetails(order, cart);

            // 3. Clean up the temporary session variables
            HttpContext.Session.Remove("Order");
            HttpContext.Session.Remove("OrderCart");

            return View(order);
        }
        // In HomeController.cs

        private void SaveOrderAndDetails(Order order, List<Product> cart)
        {
            // 1. Save the main Order
            _orderRepo.Add(order);
            _orderRepo.Save();

            // 2. Loop through the cart and create Order_Detail records
            foreach (var item in cart)
            {
                var detail = new Order_Detail
                {
                    // Order_Fid is populated when the main Order is saved
                    Order_Fid = order.Order_Id,
                    Product_Fid = item.Product_Id,
                    Quantity = item.Prod_Quantity,
                    // Capture the price at the time of the sale
                    Sale_Price = item.Product_SalePrice,
                    Purchase_Price = item.Product_PurchasePrice
                };
                _order_detail.Add(detail);
            }

            // 3. Save all the Order Details
            _order_detail.Save();

            // 4. Clear the session cart after successful booking
            HttpContext.Session.Remove("mycart");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
