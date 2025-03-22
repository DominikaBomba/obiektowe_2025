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
    public enum UserRole
    {
        Admin, Customer, Waiter, Chef
    }
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


            /*
            PasswordManager.SavePassword("a", "a");
            PasswordManager.SavePassword("Waiter", "managerPassword");
            PasswordManager.SavePassword("Customer", "userPassword");
            PasswordManager.SavePassword("Chef", "chefPassword");
            */
            PasswordManager.AddUser("a", "a", UserRole.Admin);
            PasswordManager.AddUser("waiter", "a", UserRole.Waiter);
            PasswordManager.AddUser("chef", "a", UserRole.Chef);
            PasswordManager.AddUser("customer", "a", UserRole.Customer);
            PasswordManager.AddUser("customer2", "a", UserRole.Customer);

            

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
                
                var user = new User(username, PasswordManager.GetUserRoles(username));
                
                var rbacSystem = new RBAC();

                

                while (loggedIn)
                {
                    List<string> menuOptions = new List<string>();
                    Dictionary<int, Action> menuActions = new Dictionary<int, Action>();

                    int optionNumber = 1; // Dynamic numbering

                    // Add available options dynamically and map actions
                    if (rbacSystem.HasPermission(user, Permission.ManageMenu))
                    {
                        menuOptions.Add("Manage menu");
                        menuActions[optionNumber++] = () => Console.WriteLine("Menu changed...");
                    }
                    if (rbacSystem.HasPermission(user, Permission.ViewMenu))
                    {
                        menuOptions.Add("View menu");
                        menuActions[optionNumber++] = () => m1.DisplayMenu();
                    }
                    if (rbacSystem.HasPermission(user, Permission.PlaceOrder))
                    {
                        menuOptions.Add("Place an order");
                        menuActions[optionNumber++] = () => Console.WriteLine("Order placed...");
                    }
                    if (rbacSystem.HasPermission(user, Permission.ChangeOrderStatus))
                    {
                        menuOptions.Add("Change order status");
                        menuActions[optionNumber++] = () => Console.WriteLine("Order status changed...");
                    }
                    if (rbacSystem.HasPermission(user, Permission.ViewLogs))
                    {
                        menuOptions.Add("View logs");
                        menuActions[optionNumber++] = () => Console.WriteLine("Logs viewed...");
                    }
                    if (rbacSystem.HasPermission(user, Permission.ProcessPayments))
                    {
                        menuOptions.Add("Process payments");
                        menuActions[optionNumber++] = () => Console.WriteLine("Payments processed...");
                    }
                    if (rbacSystem.HasPermission(user, Permission.ViewOrders))
                    {
                        menuOptions.Add("View orders");
                        menuActions[optionNumber++] = () => Console.WriteLine("Orders viewed...");
                    }
                    if (rbacSystem.HasPermission(user, Permission.ServeOrder))
                    {
                        menuOptions.Add("Serve order");
                        menuActions[optionNumber++] = () => Console.WriteLine("Order served...");
                    }
                    if (rbacSystem.HasPermission(user, Permission.CheckOrderStatus))
                    {
                        menuOptions.Add("Check order status");
                        menuActions[optionNumber++] = () => Console.WriteLine("Status shown...");
                    }

                    // Always available options
                    menuOptions.Add("Log out");
                    menuActions[optionNumber++] = () =>
                    {
                        Console.WriteLine("Logged out");
                        loggedIn = false;
                    };

                    menuOptions.Add("Exit the program");
                    menuActions[optionNumber++] = () =>
                    {
                        Console.WriteLine("Closing the program");
                        Environment.Exit(0);
                    };

                    // Display menu options
                    Console.Clear();
                    Console.WriteLine($"Logged in as: {username}");
                    Console.WriteLine("\nChoose an option:");
                    for (int i = 0; i < menuOptions.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {menuOptions[i]}");
                    }

                    // Get user choice
                    int choice;
                    if (!int.TryParse(Console.ReadLine(), out choice) || !menuActions.ContainsKey(choice))
                    {
                        Console.WriteLine("Invalid choice. Please try again.");
                    }
                    else
                    {
                        // Execute the selected action
                        menuActions[choice]();
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
