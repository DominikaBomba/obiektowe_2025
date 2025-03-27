using projekt_restauracja.Models;
using projekt_restauracja.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console; //by ładne tabelki były


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
            OrderManager orderManager = new OrderManager();

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // Wyjście dwa poziomy wyżej: bin\Debug\ -> projekt_restauracja\ -> projekt_restauracja\
            string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.FullName;
            string filePath = Path.Combine(projectDirectory, "Data", "dishes.txt");

            Menu m1 = new Menu(filePath);

            PasswordManager.AddUser("a", "a", UserRole.Admin);
            PasswordManager.AddUser("waiter", "a", UserRole.Waiter);
            PasswordManager.AddUser("chef", "a", UserRole.Chef);
            PasswordManager.AddUser("customer", "a", UserRole.Customer);
            PasswordManager.AddUser("customer2", "a", UserRole.Customer);
            PasswordManager.AddUser("customer3", "a", UserRole.Customer);

            bool exitProgram = false;
            bool loggedIn = true;

            while (!exitProgram)
            {
                Console.Clear();


                Console.WriteLine("=== aaRESTAURANT ===");

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




                var order1 = new Order("customer1");
                order1.AddDish(new Dish("Pizza", 25.99f, Category.Main));
                order1.AddDish(new Dish("Coke", 5.50f, Category.Beverage));
                order1.AddDish(new Dish("Pasta", 30.00f, Category.Main));

                orderManager.AddOrder(order1);

                // ✅ Kucharz gotuje
                order1.MarkAsCooked();

                // ✅ Kelner podaje
                order1.MarkAsServed();

                // ✅ Kasjer oznacza jako zapłacone
                order1.MarkAsPaid();

                // 🖥️ Wyświetlenie zamówienia w tabeli
                order1.DisplayOrder();

                var rbacSystem = new RBAC();
                Console.ReadKey();
                while (loggedIn)
                {
                    List<string> menuOptions = new List<string>();
                    Dictionary<int, Action> menuActions = new Dictionary<int, Action>();

                    int optionNumber = 1; // Dynamic numbering

                    // Add available options dynamically and map actions
                    if (rbacSystem.HasPermission(user, Permission.ManageMenu))
                    {
                        menuOptions.Add("Manage menu");
                        menuActions[optionNumber++] = () => ManageMenu(m1); // Manage menu option
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
                    // Wyświetlanie opcji menu w tabelce
                    Console.Clear();
                    Console.WriteLine($"Logged in as: {username}");
                    Console.WriteLine("\nChoose an option:");

                    // Tworzymy tabelę
                    var table = new Table();
                    table.AddColumn("Option");
                    table.AddColumn("Description");

                    // Dodajemy wiersze do tabeli
                    int i = 1;
                    foreach (var menuOption in menuOptions)
                    {
                        table.AddRow(i.ToString(), menuOption);  // Numer opcji i jej opis
                        i++;
                    }

                    // Renderujemy tabelę
                    AnsiConsole.Render(table);

                    // Pobieramy wybór użytkownika
                    int choice;
                    if (!int.TryParse(Console.ReadLine(), out choice) || !menuActions.ContainsKey(choice))
                    {
                        Console.WriteLine("Invalid choice. Please try again.");
                    }
                    else
                    {
                        // Wykonujemy wybraną akcję
                        menuActions[choice]();
                    }

                    Console.ReadKey();
                }
            
            }
        }

        static void ManageMenu(Menu m1)
        {
            bool manageMenu = true;
            while (manageMenu)
            {
                Console.Clear();
                Console.WriteLine("=== Manage Menu ===");
                Console.WriteLine("1. Add a dish");
                Console.WriteLine("2. Remove a dish");
                Console.WriteLine("3. Modify a dish price");
                Console.WriteLine("4. Back to main menu");

                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        // Add dish
                        Console.Write("Enter dish name: ");
                        string name = Console.ReadLine();

                        // Check if the dish already exists
                        if (m1.Dishes.Any(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                        {
                            Console.WriteLine("Dish with this name already exists. Please choose another name.");
                            break;
                        }

                        Console.Write("Enter dish price: ");
                        float price = float.Parse(Console.ReadLine(), CultureInfo.InvariantCulture.NumberFormat);
                        Console.WriteLine("Choose a category (0: Appetizer, 1: Main, 2: Dessert, 3: Beverage): ");
                        int categoryChoice = int.Parse(Console.ReadLine());
                        m1.AddDish(name, price, (Category)categoryChoice);
                        break;

                    case 2:
                        // Remove dish
                        m1.DisplayMenu();
                        Console.Write("Enter the name of the dish to remove: ");
                        string dishNameToRemove = Console.ReadLine();
                        if (!m1.Dishes.Any(d => d.Name.Equals(dishNameToRemove, StringComparison.OrdinalIgnoreCase)))
                        {
                            Console.WriteLine("Dish not found. Please ensure you entered the correct name.");

                            do
                            {
                                Console.Write("Enter the name of the dish to remove: ");
                                dishNameToRemove = Console.ReadLine();
                                
                            } while (!m1.Dishes.Any(d => d.Name.Equals(dishNameToRemove, StringComparison.OrdinalIgnoreCase)));
                        }

                            m1.RemoveDish(dishNameToRemove);
                        break;

                    case 3:
                        // Modify price
                        Console.Write("Enter the name of the dish to modify: ");
                        string dishNameToModify = Console.ReadLine();

                        // Check if the dish exists
                        if (!m1.Dishes.Any(d => d.Name.Equals(dishNameToModify, StringComparison.OrdinalIgnoreCase)))
                        {
                            Console.WriteLine("Dish not found. Please ensure you entered the correct name.");
                            break;
                        }

                        Console.Write("Enter the new price: ");
                        float newPrice = float.Parse(Console.ReadLine(), CultureInfo.InvariantCulture.NumberFormat);
                        m1.ModifyPrice(dishNameToModify, newPrice);
                        break;

                    case 4:
                        // Back to main menu
                        manageMenu = false;
                        break;

                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
    }
}
