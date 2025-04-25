Authors: Dominika Bomba and Nikodem Jokiel
DOCUMENTATION -> 📄 [View Documentation (PDF)](Documentation.pdf)

Console-Based Restaurant Application
Object-oriented application developed in C# with .NET 8.0.
It enables user management, order handling, order status tracking, and revenue summary generation.

-----Features used-----
delegates, events, interfaces, class inheritance,
RBAC system, password hashing, file saving.

----Tests----
Unit tests were implemented using xUnit.

----Features----
User registration and login with roles: Admin, Customer, Waiter, Cook
Placing, cooking, serving, and paying for orders
Notifications for waiters and cooks
Revenue table and order chart
Support for multiple order statuses (Placed, Cooked, Served, Paid)

----How to Run----
Open the project in Visual Studio
Make sure .NET 8.0 is installed
Run the project (F5 or Ctrl+F5)

----Project Structure----
Models – data classes (e.g., Order, User, OrderManager)
Services – logic management, e.g., UserManager, LogManager
Program.cs – application entry point
Tests – unit tests created using xUnit

----Libraries----
Spectre.Console – aesthetic tables, charts, and colors in the console
xUnit – for unit testing key functionalities

