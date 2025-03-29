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

            while (!exitProgram)
            {
                Console.Clear();
                Console.WriteLine("=== RESTAURANT ===");

                // Main menu options
                var mainMenu = new SelectionPrompt<string>()
                    .Title("[green]Please select an option:[/]")
                    .AddChoices("Sign Up", "Log In", "Exit");

                string selectedOption = AnsiConsole.Prompt(mainMenu);

                switch (selectedOption)
                {
                    case "Sign Up":
                        // Sign Up
                        SignUp();
                        break;

                    case "Log In":
                        // Log In
                        LogIn();
                        break;

                    case "Exit":
                        exitProgram = true;
                        Console.WriteLine("Exiting the program...");
                        Environment.Exit(0); // Exit the program
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }

        }
        private static void SignUp()
        {
            Console.Clear();
            Console.WriteLine("=== SIGN UP ===");

            string username = AnsiConsole.Ask<string>("Enter username: ");
            string password = AnsiConsole.Ask<string>("Enter password: ");

            // Add the user with a default role of "Customer"
            PasswordManager.AddUser(username, password, UserRole.Customer);
            Console.WriteLine($"User {username} created successfully with the Customer role.");
            Console.ReadKey();
        }

        private static void LogIn()
        {
            Console.Clear();
            Console.WriteLine("=== LOG IN ===");

            string username = AnsiConsole.Ask<string>("Enter username: ");
            string password = AnsiConsole.Ask<string>("Enter password: ");

            // Verify if the credentials are correct
            if (!PasswordManager.VerifyPassword(username, password))
            {
                Console.WriteLine("Incorrect username or password. Please try again.");
                Console.ReadKey();
                return; // Go back to main menu after failed login
            }

            // Create user object and set role
            
            var user = new User(username, PasswordManager.GetUserRoles(username));
            
            Console.ReadKey();

            // After logging in, you can show the user-specific menu
            ShowUserMenu(user);
        }

        private static void ShowUserMenu(User user)
        {
            bool loggedIn = true;
            var orderManager = new OrderManager();
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.FullName;
            string filePath = Path.Combine(projectDirectory, "Data", "dishes.txt");
            Menu m1 = new Menu(filePath);

            while (loggedIn)
            {
                var rbacSystem = new RBAC();
                var menuOptions = new List<string>();
                var menuActions = new Dictionary<int, Action>();
                Console.Clear();
                Console.WriteLine($"You're logged in as {user.Username}!");

                int optionNumber = 1;

                // Add options based on user roles
                if (rbacSystem.HasPermission(user, Permission.ManageMenu) || rbacSystem.HasPermission(user, Permission.ViewMenu))
                {
                    menuOptions.Add("Menu");
                    menuActions[optionNumber++] = () => OptionMenu(m1, user, rbacSystem);
                }
                if (rbacSystem.HasPermission(user, Permission.DisplayOrders) || rbacSystem.HasPermission(user, Permission.PlaceAnOrder))
                {
                    menuOptions.Add("Orders");
                    menuActions[optionNumber++] = () => OrderOption(user, rbacSystem, orderManager);
                }
                if (rbacSystem.HasPermission(user, Permission.ManageEmployees))
                {
                    menuOptions.Add("Employees");
                    menuActions[optionNumber++] = () => EmployeeOption(user, rbacSystem);
                }
                if (rbacSystem.HasPermission(user, Permission.ManageClients))
                {
                    menuOptions.Add("Clients");
                    menuActions[optionNumber++] = () => CustomerOption(user, rbacSystem);
                }

                if (rbacSystem.HasPermission(user, Permission.ViewRevenue))
                {
                    menuOptions.Add("Revenues");
                    menuActions[optionNumber++] = () => Console.WriteLine("revenue viewed...");
                }

                // Always available options
                menuOptions.Add("Log out");
                menuActions[optionNumber++] = () => {
                    Console.WriteLine("Logged out");
                    loggedIn = false;
                };

                menuOptions.Add("Exit the program");
                menuActions[optionNumber++] = () => {
                    Console.WriteLine("Closing the program");
                    Environment.Exit(0);
                };

                // Show user menu options using AnsiConsole SelectionPrompt
                var selectionPrompt = new SelectionPrompt<string>()
                    .Title("[green]Select an option:[/]")
                    .AddChoices(menuOptions);

                string selectedOption = AnsiConsole.Prompt(selectionPrompt);

                // Execute corresponding action
                if (menuOptions.Contains(selectedOption))
                {
                    menuActions[menuOptions.IndexOf(selectedOption) + 1](); // Menu options start from 1
                }

                Console.ReadKey();
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

                        Console.Write("Enter the name of the dish you want to order: ");
                        string selectedDishName = Console.ReadLine();

                        if (!m.Dishes.Any(d => d.Name.Equals(selectedDishName, StringComparison.OrdinalIgnoreCase)))
                        {
                            Console.WriteLine("Dish not found. Please ensure you entered the correct name.");
                            return;
                        }

                        newOrder.AddDish(m.GetDish(selectedDishName));
                        orderManager.AddOrder(newOrder);
                        AnsiConsole.MarkupLine("[green]✅ New order placed![/]");
                        //AnsiConsole.Markup("[yellow](ZAWSZE DODAJE SIE MAKARON- DO ZMIANY)[/]");
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
                        if(user.Roles.Contains(UserRole.Admin))
                            orderManager.DisplayAllOrders();
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
                        AnsiConsole.Markup("[bold green]Enter dish name: [/]");

                        string name = Console.ReadLine();

                        if (m1.Dishes.Any(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                        {
                            AnsiConsole.MarkupLine("[red]⚠️ Dish with this name already exists.[/]");
                            return;
                        }
                        Console.Clear();

                        //AnsiConsole.MarkupLine("[bold green]Enter dish price: [/]");

                        float price = GetValidFloat("Enter dish price: ");
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

                        int categoryChoice;
                        AnsiConsole.Markup("[green]Select a category (0-3): [/]");

                        while (!int.TryParse(Console.ReadLine(), out categoryChoice) || categoryChoice < 0 || categoryChoice > 3)
                        {
                            AnsiConsole.MarkupLine("[red]Invalid input. Please enter a number between 0 and 3.[/]");
                            AnsiConsole.Markup("[green]Select a category (0-3): [/]");
                        }

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
                        AnsiConsole.Markup("[bold green]Enter the name of the dish to remove: [/]");
                        //Console.Write("Enter the name of the dish to remove: ");
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

                        //Console.Write("Enter the new price: ");
                        float newPrice = GetValidFloat("Enter the new price: ");
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

                int choice = AnsiConsole.Ask<int>("\nChoose an option: ");

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

        static void EmployeeOption(User user, RBAC rbacSystem)
        {
            bool manageEmployees = true;

            while (manageEmployees)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[bold]👥 Employee Management 👥[/]");

                Dictionary<int, Action> employeeActions = new Dictionary<int, Action>();
                int optionNumber = 1;

                var table = new Table();
                table.Border = TableBorder.Rounded;
                table.AddColumn("[yellow]Option[/]");
                table.AddColumn("[yellow]Description[/]");

                // Display Employees
                table.AddRow(optionNumber.ToString(), "Display all employees");
                employeeActions[optionNumber++] = () =>
                {
                    Console.Clear();
                    PasswordManager.DisplayAllEmployees();
                };
                // Add Employee
                table.AddRow(optionNumber.ToString(), "Add a new employee");
                employeeActions[optionNumber++] = () =>
                {
                    Console.Clear();
                    string newUsername = AnsiConsole.Ask<string>("Enter new employee username: ");
                    string newPassword = AnsiConsole.Ask<string>("Enter new employee password: ");
                    UserRole newRole = AnsiConsole.Prompt(
                        new SelectionPrompt<UserRole>()
                            .Title("Select a role for the new employee:")
                            .AddChoices(UserRole.Chef, UserRole.Waiter)
                    );

                    PasswordManager.AddUser(newUsername, newPassword, newRole);
                    AnsiConsole.MarkupLine($"[green]✅ Employee {newUsername} added successfully as {newRole}![/]");
                };

                // Fire Employee
                table.AddRow(optionNumber.ToString(), "Fire an employee");
                employeeActions[optionNumber++] = () =>
                {
                    Console.Clear();
                    PasswordManager.DisplayAllEmployees();
                    string usernameToRemove = AnsiConsole.Ask<string>("Enter the username of the employee to fire: ");

                    List<string> lines = File.ReadAllLines(PasswordManager.GetRoleFilePath()).ToList();
                    if (lines.RemoveAll(line => line.StartsWith(usernameToRemove + ",")) > 0)
                    {
                        File.WriteAllLines(PasswordManager.GetRoleFilePath(), lines);
                        AnsiConsole.MarkupLine($"[red]❌ Employee {usernameToRemove} has been removed.[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"[red]Employee {usernameToRemove} not found.[/]");
                    }
                };

                // Add Role to Employee
                table.AddRow(optionNumber.ToString(), "Add a role to an employee");
                employeeActions[optionNumber++] = () =>
                {
                    Console.Clear();
                    PasswordManager.DisplayAllEmployees();
                    string username = AnsiConsole.Ask<string>("Enter the username to add a role to: ");
                    UserRole newRole = AnsiConsole.Prompt(
                        new SelectionPrompt<UserRole>()
                            .Title("Select a role to add:")
                            .AddChoices(Enum.GetValues(typeof(UserRole)).Cast<UserRole>())
                    );

                    PasswordManager.AddUserRole(username, newRole);
                    AnsiConsole.MarkupLine($"[green]✅ Role {newRole} added to {username}![/]");
                };

                // Remove Role from Employee
                table.AddRow(optionNumber.ToString(), "Remove a role from an employee");
                employeeActions[optionNumber++] = () =>
                {
                    Console.Clear();
                    PasswordManager.DisplayAllEmployees();
                    string username = AnsiConsole.Ask<string>("Enter the username to remove a role from: ");
                    List<UserRole> roles = PasswordManager.GetUserRoles(username);

                    if (roles.Count == 0)
                    {
                        AnsiConsole.MarkupLine("[red]User has no roles to remove![/]");
                        return;
                    }

                    UserRole roleToRemove = AnsiConsole.Prompt(
                        new SelectionPrompt<UserRole>()
                            .Title("Select a role to remove:")
                            .AddChoices(roles)
                    );

                    PasswordManager.RemoveUserRole(username, roleToRemove);
                    AnsiConsole.MarkupLine($"[yellow]⚠️ Role {roleToRemove} removed from {username}.[/]");
                };

                // Exit to Main Menu
                table.AddRow(optionNumber.ToString(), "Back to main menu");
                employeeActions[optionNumber++] = () =>
                {
                    manageEmployees = false;
                    AnsiConsole.MarkupLine("[yellow]↩️ Returning to main menu...[/]");
                };

                AnsiConsole.Render(table);

                int choice = AnsiConsole.Ask<int>("\nChoose an option: ");
                if (employeeActions.ContainsKey(choice))
                {
                    employeeActions[choice]();
                }
                else
                {
                    AnsiConsole.Markup("[red]Invalid choice. Try again.[/]");
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        static void CustomerOption(User user, RBAC rbacSystem)
        {
            bool manageCustomers = true;

            while (manageCustomers)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[bold]👥 Customer Management 👥[/]");

                Dictionary<int, Action> customerActions = new Dictionary<int, Action>();
                int optionNumber = 1;

                var table = new Table();
                table.Border = TableBorder.Rounded;
                table.AddColumn("[yellow]Option[/]");
                table.AddColumn("[yellow]Description[/]");

                // Display Customers
                table.AddRow(optionNumber.ToString(), "Display all customers");
                customerActions[optionNumber++] = () =>
                {
                    Console.Clear();
                    PasswordManager.DisplayCustomers();
                };

                // Add Customer
                table.AddRow(optionNumber.ToString(), "Add a customer");
                customerActions[optionNumber++] = () =>
                {
                    Console.Clear();
                    string username = AnsiConsole.Ask<string>("Enter new customer's username: ");
                    string password = AnsiConsole.Ask<string>("Enter new customer's password: ");

                    // Use AddUser method with UserRole.Customer
                    PasswordManager.AddUser(username, password, UserRole.Customer);
                };

                // Remove Customer
                table.AddRow(optionNumber.ToString(), "Remove a customer");
                customerActions[optionNumber++] = () =>
                {
                    Console.Clear();
                    PasswordManager.DisplayCustomers();
                    string usernameToRemove = AnsiConsole.Ask<string>("Enter the username of the customer to remove: ");

                    List<string> lines = File.ReadAllLines(PasswordManager.GetRoleFilePath()).ToList();
                    if (lines.RemoveAll(line => line.StartsWith(usernameToRemove + ",")) > 0)
                    {
                        File.WriteAllLines(PasswordManager.GetRoleFilePath(), lines);
                        AnsiConsole.MarkupLine($"[red]❌ Customer {usernameToRemove} has been removed.[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"[red]Customer {usernameToRemove} not found.[/]");
                    }
                };

                // Exit to Main Menu
                table.AddRow(optionNumber.ToString(), "Back to main menu");
                customerActions[optionNumber++] = () =>
                {
                    manageCustomers = false;
                    AnsiConsole.MarkupLine("[yellow]↩️ Returning to main menu...[/]");
                };

                AnsiConsole.Render(table);

                int choice = AnsiConsole.Ask<int>("\nChoose an option: ");
                if (customerActions.ContainsKey(choice))
                {
                    customerActions[choice]();
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Invalid choice. Try again.[/]");
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
        static float GetValidFloat(string message)
        {
            float value;
            AnsiConsole.Markup($"[bold green]{message} [/]");

            while (!float.TryParse(Console.ReadLine(), NumberStyles.Float, CultureInfo.InvariantCulture, out value) || value < 0)
            {
                Console.WriteLine("Invalid input. Please enter a valid positive number.");
                AnsiConsole.Markup($"[bold green]{message} [/]");
            }

            return value;
        }
    }

    }

