using projekt_restauracja.Models;
using projekt_restauracja.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt_restauracja
{
    class Program
    {
        static void Main()
        {

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // Wyjście dwa poziomy wyżej: bin\Debug\ -> projekt_restauracja\ -> projekt_restauracja\
            string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.FullName;
            string filePath = Path.Combine(projectDirectory, "Data", "dishes.txt");

            Menu m1 = new Menu(filePath);
            


            //RBAC rbacSystem = new RBAC();

            PasswordManager.SavePassword("AdminUser", "adminPassword");
            PasswordManager.SavePassword("Waiter", "managerPassword");
            PasswordManager.SavePassword("Customer", "userPassword");
            PasswordManager.SavePassword("Chef", "chefPassword");
            

            bool exitProgram = false;
            bool loggedIn = true;

            while (!exitProgram)
            {
                Console.Clear();
                Console.WriteLine("=== RESTAURANT ===");

                Console.Write("Enter username: ");
                string username = Console.ReadLine();

                Console.Write("Enter password: ");
                string password = Console.ReadLine();

                if (!PasswordManager.VerifyPassword(username, password))
                {
                    Console.WriteLine("Incorrect username or password");
                    Console.ReadKey();
                    continue;
                }

                loggedIn = true;

                var user = new User(username);
                if (username == "AdminUser") user.AddRole(UserRole.Admin);
                else if (username == "Waiter") user.AddRole(UserRole.Waiter);
                else if (username == "Customer") user.AddRole(UserRole.Customer);
                else if (username == "Chef") user.AddRole(UserRole.Chef); 

                var rbacSystem = new RBAC();


                while (loggedIn)
                {
                    Console.Clear();
                    Console.WriteLine($"Logged in as: {username}");
                    Console.WriteLine("\nChoose an option:");
                    Console.WriteLine("1. View menu");
                    if (rbacSystem.HasPermission(user, Permission.PlaceOrder))
                        Console.WriteLine("2. Place an order");
                    if (rbacSystem.HasPermission(user, Permission.ChangeOrderStatus))
                        Console.WriteLine("3. Change order status");
                    if (rbacSystem.HasPermission(user, Permission.ViewLogs))
                        Console.WriteLine("4. View logs");
                    if (rbacSystem.HasPermission(user, Permission.ProcessPayments))
                        Console.WriteLine("5. Process payments");
                    if (rbacSystem.HasPermission(user, Permission.ViewOrders))
                        Console.WriteLine("6. View orders");
                    if (rbacSystem.HasPermission(user, Permission.ServeOrder))
                        Console.WriteLine("7. Serve order");
                    Console.WriteLine("8. Log out");
                    Console.WriteLine("9. Exit the program");

                    int choice;
                    if (!int.TryParse(Console.ReadLine(), out choice))
                    {
                        Console.WriteLine("Invalid choice. Please try again.");
                        continue;
                    }

                    switch (choice)
                    {
                        case 1:
                            if (rbacSystem.HasPermission(user, Permission.ViewMenu))
                                m1.DisplayMenu();
                            else
                                Console.WriteLine("You don't have permission to view the menu.");
                            break;
                        case 2:
                            if (rbacSystem.HasPermission(user, Permission.PlaceOrder))
                                Console.WriteLine("Order placed...");
                            else
                                Console.WriteLine("You don't have permission to place an order.");
                            break;
                        case 3:
                            if (rbacSystem.HasPermission(user, Permission.ChangeOrderStatus))
                                Console.WriteLine("Order status changed...");
                            else
                                Console.WriteLine("You don't have permission to change the order status.");
                            break;
                        case 4:
                            if (rbacSystem.HasPermission(user, Permission.ViewLogs))
                                Console.WriteLine("Logs viewed...");
                            else
                                Console.WriteLine("You don't have permission to view the logs.");
                            break;
                        case 5:
                            if (rbacSystem.HasPermission(user, Permission.ProcessPayments))
                                Console.WriteLine("Payments processed...");
                            else
                                Console.WriteLine("You don't have permission to process payments.");
                            break;
                        case 6:
                            if (rbacSystem.HasPermission(user, Permission.ViewOrders))
                                Console.WriteLine("Orders viewed...");
                            else
                                Console.WriteLine("You don't have permission to view orders.");
                            break;
                        case 7:
                            if (rbacSystem.HasPermission(user, Permission.ServeOrder))
                                Console.WriteLine("Order served...");
                            else
                                Console.WriteLine("You don't have permission to serve orders.");
                            break;
                        case 8:
                            Console.WriteLine("Logged out");
                            loggedIn = false;
                            break;
                        case 9:
                            Console.WriteLine("Closing the program");
                            return;
                        default:
                            Console.WriteLine("Invalid choice");
                            break;
                    }

                    Console.ReadKey();
                }


                /*
                // Create example users (only if they don't exist)
                CreateTestUsers();

                Console.WriteLine("Welcome to the Restaurant Management System!");
                Console.Write("Enter username: ");
                string username = Console.ReadLine();
                Console.Write("Enter password: ");
                string password = Console.ReadLine();

                User user = authService.Authenticate(username, password);
                if (user != null)
                {
                    Console.WriteLine($"Login successful! Welcome, {user.Username}.");
                    ShowMenu(user, rbac);
                }
                else
                {
                    Console.WriteLine("Invalid username or password.");
                }*/
            }


        }


    }
}
