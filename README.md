🛍️ The Way Shop (E-commerce Project)

A robust and feature-rich e-commerce web application built on ASP.NET Core MVC (C#). This project provides a full-stack solution including secure administration, product and category management, a customer-facing shop, and an integrated shopping cart and payment gateway.

🌟 Key Features

The application is structured to support both a public-facing customer store and a secure administrative dashboard.

🛒 Customer Facing

Product Catalog: Browse and view detailed information for products across various categories.

Shopping Cart: Session-based functionality to add, view, and remove items from the cart.

User Authentication: Secure sign-up and login for customer accounts.

Payment Integration: Seamless checkout process with PayPal Sandbox integration for processing orders.

General Pages: Dedicated views for About, Contact, and Order Confirmation.

⚙️ Administration Panel

Secure Admin Dashboard: A separate administrative interface protected by session-based authentication.

Product Management: Full CRUD (Create, Read, Update, Delete) operations for product listings, including image file uploads.

Category Management: Full CRUD operations for organizing and managing product categories.

Admin User Management: Full CRUD operations for managing administrative users/accounts.

Dedicated Layout: A distinct, modern admin dashboard layout (_LayoutAdmin.cshtml) for improved management experience.

🔹 Architectural Improvements

Refactored Controllers to Use Generic Repositories:
All CRUD operations in controllers no longer directly depend on Entity Framework Core.
Instead, controllers now utilize Generic Repositories, which handle database operations through a reusable, decoupled interface.

Benefits of Generic Repositories:

Reduces repetitive code across multiple controllers.

Makes unit testing easier by allowing mock repositories instead of a real database.

Supports a clean, scalable, and maintainable architecture for future development.

🛠️ Technologies Used

Backend: ASP.NET Core MVC (C#)

Database: Entity Framework Core (EF Core)

Authentication: Session-based Authentication/Authorization (Utilizing IHttpContextAccessor)

---

State Management: Session Storage (for Shopping Cart: mycart)

Payment Gateway: PayPal Sandbox

---

## 🗂️ Project Structure  

### 🔹 User Admin
![User Admin](Screenshot/UserAdmin.png)

### 🔹 Dashboard
![Dashboard](Screenshot/Dashboard.png)

### 🔹 Carts
![Carts](Screenshot/Carts.png)

### 🔹 Products-Bag
![Products-Bag](Screenshot/Products-Bag.png)

### 🔹 Products
![Products](Screenshot/Products.png)

### 🔹 All-Products
![All-Products](Screenshot/All-Products.png)

