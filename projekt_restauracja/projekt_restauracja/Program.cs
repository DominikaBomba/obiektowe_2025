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
    public enum OrderStatus { Placed, Cooked, Served, Paid }

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




                var order1 = new Order("customer1");
                order1.AddDish(new Dish("Pizza", 25.99f, Category.Main));
                order1.AddDish(new Dish("Coke", 5.50f, Category.Beverage));
                order1.AddDish(new Dish("Pasta", 30.00f, Category.Main));



                Console.OutputEncoding = System.Text.Encoding.UTF8;
                Console.WriteLine("\uD83C\uDF74");
                var rbacSystem = new RBAC();
                Console.ReadKey();
                while (loggedIn)
                {
                    List<string> menuOptions = new List<string>();
                    Dictionary<int, Action> menuActions = new Dictionary<int, Action>();

                    int optionNumber = 1;


                    //menu - can be seen by clients and admin
                    if (rbacSystem.HasPermission(user, Permission.ManageMenu) || rbacSystem.HasPermission(user, Permission.ViewMenu))
                    {
                        menuOptions.Add("Menu");
                        menuActions[optionNumber++] = () => OptionMenu(m1, user, rbacSystem);
                    }
                    //orders - can be seen by admin, client, waiter, chef
                    if (rbacSystem.HasPermission(user, Permission.DisplayOrders) || //admin
                        rbacSystem.HasPermission(user, Permission.ChangeOrderStatus) || //chef and waiter
                        rbacSystem.HasPermission(user, Permission.PlaceAnOrder)  //client
                        )
                    {
                        menuOptions.Add("Orders");
                        menuActions[optionNumber++] = () => OrderOption(user, rbacSystem, orderManager);
                    }

                    //employees - can only be seen by admin
                    if (rbacSystem.HasPermission(user, Permission.ManageEmployees))
                    {
                        menuOptions.Add("Employees");
                        menuActions[optionNumber++] = () => Console.WriteLine("employees");
                    }

                    //manage clients
                    if (rbacSystem.HasPermission(user, Permission.ManageClients))
                    {
                        menuOptions.Add("Clients");
                        menuActions[optionNumber++] = () => Console.WriteLine("Clients");
                    }

                    //revenues
                    if (rbacSystem.HasPermission(user, Permission.ViewRevenue))
                    {
                        menuOptions.Add("Revenues");
                        menuActions[optionNumber++] = () => Console.WriteLine("revenue viewed...");
                    }

                    //logs
                    if (rbacSystem.HasPermission(user, Permission.ViewLogs))
                    {
                        menuOptions.Add("Logs");
                        menuActions[optionNumber++] = () => Console.WriteLine("Logs viewed...");
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
                    Console.OutputEncoding = System.Text.Encoding.UTF8;
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




        static void OrderOption(User user, RBAC rbacSystem, OrderManager orderManager)
        {
            bool manageOrders = true;

            while (manageOrders)
            {
                Console.Clear();

                AnsiConsole.MarkupLine("[bold]🍽️ Manage Orders 🍽️[/]");

                Dictionary<int, Action> orderActions = new Dictionary<int, Action>();
                int orderOptionNumber = 1;

                var table = new Table();
                table.Border = TableBorder.Rounded;
                table.AddColumn("[yellow]Option[/]");
                table.AddColumn("[yellow]Description[/]");

                if (rbacSystem.HasPermission(user, Permission.DisplayOrders))
                {
                    table.AddRow(orderOptionNumber.ToString(), "Display all orders");
                    orderActions[orderOptionNumber++] = () => orderManager.DisplayAllOrders();
                }

                if (rbacSystem.HasPermission(user, Permission.PlaceAnOrder))
                {
                    table.AddRow(orderOptionNumber.ToString(), "Place an Order");
                    orderActions[orderOptionNumber++] = () =>
                    {
                        Console.Clear();
                        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                        string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.FullName;
                        string filePath = Path.Combine(projectDirectory, "Data", "dishes.txt");

                        Menu m = new Menu(filePath);
                        m.DisplayMenu();
                        Order newOrder = new Order(user.Username);
                        newOrder.AddDish(new Dish("makaron", 12, Category.Dessert));
                        orderManager.AddOrder(newOrder);
                        AnsiConsole.Markup("[yellow](ZAWSZE DODAJE SIE MAKARON- DO ZMIANY)[/]");
                    };
                }
                if (rbacSystem.HasPermission(user, Permission.CheckOrderStatus))
                {
                   
                    table.AddRow(orderOptionNumber.ToString(), "Check Order Status");
                    orderActions[orderOptionNumber++] = () =>
                    {
                        Console.Clear();
                        if (user.Roles.Contains(UserRole.Chef))
                            orderManager.DisplayOrdersByStatus(Order.OrderStatus.Placed);
                        if (user.Roles.Contains(UserRole.Waiter))
                            orderManager.DisplayOrdersByStatus(Order.OrderStatus.Cooked);
                    };
                }

                if (rbacSystem.HasPermission(user, Permission.ChangeOrderStatus))
                {
                    table.AddRow(orderOptionNumber.ToString(), "Change Order Status");
                    orderActions[orderOptionNumber++] = () =>
                    {
                        Console.Clear();
                        Order.OrderStatus statusToDisplay = user.Roles.Contains(UserRole.Chef) ? Order.OrderStatus.Placed : Order.OrderStatus.Cooked;
                        orderManager.DisplayOrdersByStatus(statusToDisplay);

                        int orderId = AnsiConsole.Ask<int>("Enter Order ID to change status: ");
                        var order = orderManager.GetOrderById(orderId);
                        if (order != null)
                        {
                            if (user.Roles.Contains(UserRole.Chef))
                                order.MarkAsCooked();
                            else if (user.Roles.Contains(UserRole.Waiter))
                                order.MarkAsServed();
                            AnsiConsole.Markup("[green]Order status updated![/]");
                        }
                        else
                            AnsiConsole.Markup("[red]Order not found.[/]");
                    };
                }

                if (rbacSystem.HasPermission(user, Permission.CheckMyOrders))
                {
                    table.AddRow(orderOptionNumber.ToString(), "Check My Orders");
                    orderActions[orderOptionNumber++] = () => orderManager.DisplayOrdersByUserId(user.Username);
                }

                if (rbacSystem.HasPermission(user, Permission.PayForOrder))
                {
                    table.AddRow(orderOptionNumber.ToString(), "Pay for Order");
                    orderActions[orderOptionNumber++] = () =>
                    {
                        Console.Clear();
                        orderManager.DisplayOrdersByUserIdAndStatus(user.Username, Order.OrderStatus.Served);
                        int orderId = AnsiConsole.Ask<int>("Enter Order ID to pay: ");
                        var order = orderManager.GetOrderById(orderId);
                        if (order != null)
                        {
                            order.MarkAsPaid();
                            AnsiConsole.Markup("[green]Order paid![/]");
                        }
                        else
                            AnsiConsole.Markup("[red]Order not found or already paid.[/]");
                    };
                }
                table.AddRow(orderOptionNumber.ToString(), "Back to main menu");
                orderActions[orderOptionNumber++] = () =>
                {
                    manageOrders = false;
                    AnsiConsole.MarkupLine("[yellow]↩️ Returning to main menu...[/]");
                };

                AnsiConsole.Render(table);

                int choice = AnsiConsole.Ask<int>("\nChoose an option: ");
                if (orderActions.ContainsKey(choice))
                {
                    orderActions[choice]();
                }
                else
                {
                    AnsiConsole.Markup("[red]Invalid choice. Try again.[/]");
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }


        static void OptionMenu(Menu m1, User user, RBAC rbacSystem)
        {
            bool manageMenu = true;

            while (manageMenu)
            {
                Console.Clear();

                // Nagłówek menu
                AnsiConsole.MarkupLine("[bold]🍽️ Manage Menu 🍽️[/]");

                Dictionary<int, Action> menuActions = new Dictionary<int, Action>();
                int optionNumber = 1;

                var table = new Table();
                table.Border = TableBorder.Rounded;
                table.AddColumn("[yellow]Option[/]");
                table.AddColumn("[yellow]Description[/]");
                if (rbacSystem.HasPermission(user, Permission.ViewMenu))
                {
                    Console.Clear();
                    table.AddRow(optionNumber.ToString(), "View a full menu");
                    menuActions[optionNumber++] = () =>
                    {
                        m1.DisplayMenu();
                    };
                }

                if (rbacSystem.HasPermission(user, Permission.ManageMenu))
                {
                    table.AddRow(optionNumber.ToString(), "Add a dish");
                    menuActions[optionNumber++] = () =>
                    {
                        Console.Clear();
                        AnsiConsole.MarkupLine("[bold green]Enter dish name: [/]");

                        string name = Console.ReadLine();

                        if (m1.Dishes.Any(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                        {
                            AnsiConsole.MarkupLine("[red]⚠️ Dish with this name already exists.[/]");
                            return;
                        }
                        Console.Clear();

                        AnsiConsole.MarkupLine("[bold green]Enter dish price: [/]");

                        float price = float.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
                        Console.Clear();

                        var table2 = new Table();
                        table2.Border = TableBorder.Rounded;
                        table2.AddColumn("[cyan]Option[/]");
                        table2.AddColumn("[green]Category[/]");

                        table2.AddRow("0", "🥗 Appetizer");
                        table2.AddRow("1", "🍽️ Main");
                        table2.AddRow("2", "🍰 Dessert");
                        table2.AddRow("3", "🥤 Beverage");

                        AnsiConsole.Render(table2);

                        int categoryChoice = int.Parse(Console.ReadLine());

                        m1.AddDish(name, price, (Category)categoryChoice);
                        AnsiConsole.MarkupLine("[green]✅ Dish added successfully![/]");
                    };
                }

                
                if (rbacSystem.HasPermission(user, Permission.ManageMenu))
                {
                    table.AddRow(optionNumber.ToString(), "Remove a dish");
                    menuActions[optionNumber++] = () =>
                    {
                        Console.Clear();
                        m1.DisplayMenu();
                        Console.Write("Enter the name of the dish to remove: ");
                        string dishNameToRemove = Console.ReadLine();

                        if (!m1.Dishes.Any(d => d.Name.Equals(dishNameToRemove, StringComparison.OrdinalIgnoreCase)))
                        {
                            Console.WriteLine("Dish not found. Please ensure you entered the correct name.");
                            return;
                        }

                        m1.RemoveDish(dishNameToRemove);
                        AnsiConsole.MarkupLine("[green]✅ Dish removed successfully![/]");
                    };
                }

                if (rbacSystem.HasPermission(user, Permission.ManageMenu))
                {
                    table.AddRow(optionNumber.ToString(), "Modify a dish price");
                    menuActions[optionNumber++] = () =>
                    {
                        Console.Clear();
                        m1.DisplayMenu();
                        Console.Write("Enter the name of the dish to modify: ");
                        string dishNameToModify = Console.ReadLine();

                        if (!m1.Dishes.Any(d => d.Name.Equals(dishNameToModify, StringComparison.OrdinalIgnoreCase)))
                        {
                            Console.WriteLine("Dish not found. Please ensure you entered the correct name.");
                            return;
                        }

                        Console.Write("Enter the new price: ");
                        float newPrice = float.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
                        m1.ModifyPrice(dishNameToModify, newPrice);
                        AnsiConsole.MarkupLine("[green]✅ Price updated successfully![/]");
                    };
                }

                
                table.AddRow(optionNumber.ToString(), "Back to main menu");
                menuActions[optionNumber++] = () =>
                {
                    manageMenu = false;
                    AnsiConsole.MarkupLine("[yellow]↩️ Returning to main menu...[/]");
                };

                AnsiConsole.Render(table);

                int choice = int.Parse(Console.ReadLine());

                if (menuActions.ContainsKey(choice))
                {
                    menuActions[choice]();
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Invalid choice. Try again.[/]");
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
    }

    }

