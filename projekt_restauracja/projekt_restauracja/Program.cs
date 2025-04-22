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
using System.Threading;

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
            Logger.Init();
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            OrderManager orderManager = new OrderManager();

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
              string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.FullName;
            string filePath = Path.Combine(projectDirectory, "Data", "dishes.txt");

            Menu m1 = new Menu(filePath);

            PasswordManager.AddUser("a", "a", UserRole.Admin);
            PasswordManager.AddUser("waiter", "a", UserRole.Waiter);
            PasswordManager.AddUser("chef", "a", UserRole.Chef);
            PasswordManager.AddUser("customer", "a", UserRole.Customer);
            PasswordManager.AddUser("customer2", "a", UserRole.Customer);
            PasswordManager.AddUser("customer3", "a", UserRole.Customer);


            bool exitProgram = false;
            //intro
            Console.Clear();
            int width = Console.WindowWidth / 10 * 6;
            var table2 = new Table().Border(TableBorder.Rounded).Width((width)); ;


            string text = "🍽️ Welcome to our restaurant 🍽️";
            table2.AddColumn("🍽️ [Orange1] Welcome to our restaurant[/] 🍽️");


            AnsiConsole.Render(table2);
            Thread.Sleep(1000);


            text = "🍕 We offer a variety of delicious dishes. ";
            Console.WriteLine($"\n\t {text}");

            Thread.Sleep(1000);
            text = "🍕 From fresh pizzas to amazing pasta";
            Console.WriteLine($"\n\t {text}");

            Console.ReadKey();
           
            Console.Clear();


            while (!exitProgram)
            {

                Console.Clear();
              
                //main menu 

                var mainMenu = new SelectionPrompt<string>()
                    .HighlightStyle("bold Orange1")
                    .AddChoices("\t📝 Sign Up \n", "\t🔑 Log In\n", "\t❌ Exit");

                string selectedOption = AnsiConsole.Prompt(mainMenu);

                switch (selectedOption)
                {
                    case "\t📝 Sign Up \n":
                        SignUp();
                        break;

                    case "\t🔑 Log In\n":
                        LogIn(orderManager);
                        break;

                    case "\t❌ Exit":
                        exitProgram = true;
                        AnsiConsole.MarkupLine("[red]Exiting the program...[/]");
                        break;

                    default:
                        AnsiConsole.MarkupLine("[red]Invalid choice. Please try again.[/]");
                        break;
                }
            }


        }
        private static void SignUp()
        {
            Console.Clear();

            var headerTable = new Table()
                .Border(TableBorder.Rounded)
                .Centered()
                .AddColumn("[bold]📝 Sign up 📝[/]");

            AnsiConsole.Render(headerTable);
            Thread.Sleep(500);

            
            string username = AnsiConsole.Ask<string>("[bold Orange1]👤 Enter username:[/]");
            string password;
            while (true)
            {
                password = AnsiConsole.Prompt(
                    new TextPrompt<string>("[bold Orange1]🔒 Enter password:[/]")
                        .PromptStyle("orange1")
                        .Secret());

                string confirmPassword = AnsiConsole.Prompt(
                    new TextPrompt<string>("[bold Orange1]🔁 Confirm password:[/]")
                        .PromptStyle("orange1")
                        .Secret());

                if (password != confirmPassword)
                {
                    AnsiConsole.MarkupLine("[red]❌ Passwords do not match. Try again.[/]");
                }
                else
                {
                    break;
                }
            }

            PasswordManager.AddUser(username, password, UserRole.Customer);


            AnsiConsole.MarkupLine("\n[grey][[Press any key to continue...]][/]");
            Console.ReadKey();
        }


        private static void LogIn(OrderManager orderManager)
        {
            Console.Clear();

            var headerTable = new Table()
                .Border(TableBorder.Rounded)
                .Centered()
                .AddColumn("[bold]📝 Log in 📝[/]");

            AnsiConsole.Render(headerTable);

            string username = AnsiConsole.Ask<string>("[bold Orange1]👤 Enter username:[/]");
            string password = AnsiConsole.Prompt(
            new TextPrompt<string>("[bold Orange1]🔒 Enter password:[/]")
                .PromptStyle("orange1")
                .Secret());

            if (!PasswordManager.VerifyPassword(username, password))
            {
                AnsiConsole.MarkupLine("\n[red]❌ Your username or password is wrong![/]");
                AnsiConsole.MarkupLine("[grey][[Press any key to continue...]][/]");
                Console.ReadKey();
                return;
            }

            var roles = PasswordManager.GetUserRoles(username);
            User user;

            if (roles.Contains(UserRole.Chef))
            {
                user = new Chef(username, roles);
            }
            else if (roles.Contains(UserRole.Waiter))
            {
                user = new Waiter(username, roles);
            }
            else
            {
                user = new User(username, roles);
                
            }

            LogManager.Log($"User '{username}' logged in.");
            AnsiConsole.MarkupLine("\n✅ Logged in!");

            
            
          

            

            Console.ReadKey();
           
            ShowUserMenu(user, orderManager);

           
        }



        private static void ShowUserMenu(User user, OrderManager orderManager)
        {
            bool loggedIn = true;
           
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.FullName;
            string filePath = Path.Combine(projectDirectory, "Data", "dishes.txt");
            Menu m1 = new Menu(filePath);

            while (loggedIn)
            {
                var rbacSystem = new RBAC();
                var menuOptions = new List<string>();
                var menuActions = new Dictionary<int, Action>();
                Console.Clear();


               var headerTable = new Table()
              .Border(TableBorder.Rounded)
              .Centered()
              .AddColumn($"[bold]📝 Logged in as: [/] {user.Username}");


                List<Order> userOrderList = orderManager.GetOrders();
                if (user.HasRole(UserRole.Customer))
                {
                    foreach (Order order in userOrderList)
                    {
                        if (order.UserId == user.Username && order.Status == Order.OrderStatus.Served)
                        {
                            headerTable.AddRow($"[bold Orange1]Order with id {order.OrderId} ready to eat[/]");
                        }

                    }
                }
                else if (user.Roles.Contains(UserRole.Chef))
                {
                    bool hasOrdersToCook = orderManager.HasOrdersWithStatus(Order.OrderStatus.Placed);
                    if (hasOrdersToCook)
                        headerTable.AddRow("[red] There are orders to cook![/]");
                    
                }
                else if (user.Roles.Contains(UserRole.Waiter))
                {
                    bool hasOrdersToServe = orderManager.HasOrdersWithStatus(Order.OrderStatus.Cooked);
                    if (hasOrdersToServe)
                        headerTable.AddRow("[red] There are orders ready to serve![/]");
                    
                }

                AnsiConsole.Render(headerTable);
                if (user is Chef chef)
                    chef.ShowNotifications(orderManager.GetNotificationsForRole(UserRole.Chef));
                else if (user is Waiter waiter)
                    waiter.ShowNotifications(orderManager.GetNotificationsForRole(UserRole.Waiter));


                int optionNumber = 1;

                // Add options based on user roles
                if (rbacSystem.HasPermission(user, Permission.ManageMenu) || rbacSystem.HasPermission(user, Permission.ViewMenu))
                {
                    menuOptions.Add("Menu");
                    menuActions[optionNumber++] = () => OptionMenu(m1, user, rbacSystem);
                }
                if (rbacSystem.HasPermission(user, Permission.DisplayOrders) || rbacSystem.HasPermission(user, Permission.PlaceAnOrder) || rbacSystem.HasPermission(user, Permission.ChangeOrderStatus))
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
                    menuActions[optionNumber++] = () => orderManager.DisplayOrderSummaryByStatus();
                }
                if (rbacSystem.HasPermission(user, Permission.ViewLogs))
                {
                    menuOptions.Add("Logs");
                    menuActions[optionNumber++] = () => Logger.DisplayLogs();
                    ;
                }


                // Always available options
                menuOptions.Add("Log out");
                menuActions[optionNumber++] = () => {
                    Console.WriteLine("Logged out");
                    if (user is Chef chefOut)
                        orderManager.ClearNotifications(UserRole.Chef);
                    else if (user is Waiter waiter)
                       orderManager.ClearNotifications(UserRole.Waiter);
                    loggedIn = false;
                   
                    LogManager.Log($"User '{user.Username}' logged out.");

                };

                menuOptions.Add("Exit the program");
                menuActions[optionNumber++] = () => {
                    Console.WriteLine("Closing the program");
                    Environment.Exit(0);
                };

                var mainMenu = new SelectionPrompt<string>()
                    .HighlightStyle("bold Orange1")
                    .AddChoices("\t📝 Sign Up \n", "\t🔑 Log In\n", "\t❌ Exit");


                var selectionPrompt = new SelectionPrompt<string>()
                 .Title("[bold Orange1]Select an option:[/]")
                 .HighlightStyle("bold Orange1")
                 .AddChoices(menuOptions.Select(option => $"{option}"));
                string selectedOption = AnsiConsole.Prompt(selectionPrompt);

                 if (menuOptions.Contains(selectedOption))
                {
                    menuActions[menuOptions.IndexOf(selectedOption) + 1](); 
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
                table.AddColumn("[Orange1]Option[/]");
                table.AddColumn("[Orange1]Description[/]");

                if (rbacSystem.HasPermission(user, Permission.DisplayOrders))
                {
                    table.AddRow(orderOptionNumber.ToString(), "Display all orders");
                    Console.Clear();
                    LogManager.Log($"User '{user.Username}' viewed all orders.");

                    orderActions[orderOptionNumber++] = () => orderManager.DisplayAllOrders();
                }

                if (rbacSystem.HasPermission(user, Permission.PlaceAnOrder))
                {
                    table.AddRow(orderOptionNumber.ToString(), "Place an Order");
                    orderActions[orderOptionNumber++] = () =>
                    {
                        Console.Clear();
                        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                        string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.FullName;
                        string filePath = Path.Combine(projectDirectory, "Data", "dishes.txt");

                        Menu m = new Menu(filePath);
                      
                        Order newOrder = new Order(user.Username);
                        LogManager.Log($"User '{user.Username}' started creating a new order.");

                        bool addingDishes = true;

                        while (addingDishes)
                        {
                            Console.WriteLine();
                            var menuPrompt = new SelectionPrompt<string>()
                                .Title("[bold]What would you like to do?[/]")
                                 .HighlightStyle("bold Orange1")
                                .AddChoices(
                                    "Add dish to your order",
                                    "Place an order"
                                );

                            string selectedOption = AnsiConsole.Prompt(menuPrompt);

                            switch (selectedOption)
                            {
                                case "Add dish to your order":
                                    Console.Clear();
                                    
                                    m.DisplayMenu();
                                    AnsiConsole.MarkupLine("\n🍽️ [bold]Enter the name of the dish you want to add:[/] ");
                                    string selectedDishName = Console.ReadLine();

                                    if (!m.Dishes.Any(d => d.Name.Equals(selectedDishName, StringComparison.OrdinalIgnoreCase)))
                                    {
                                        AnsiConsole.MarkupLine("[red]❌ Dish not found. Please ensure you entered the correct name.[/]");
                                        continue;
                                    }
                                    Console.Clear() ;
                                    newOrder.AddDish(m.GetDish(selectedDishName));
                                    LogManager.Log($"User '{user.Username}' added dish '{selectedDishName}' to the order.");
                                    AnsiConsole.MarkupLine($"✅ Dish [orange1]{selectedDishName}[/] added to your order.");
                                    break;

                                case "Place an order":
                                    if (newOrder.Dishes.Count == 0)
                                    {
                                        AnsiConsole.MarkupLine("[red]❗ You must add at least one dish before placing the order.[/]");
                                        continue;
                                    }

                                    Console.Clear();
                                    orderManager.AddOrder(newOrder);
                                    LogManager.Log($"User '{user.Username}' placed a new order.");

                                    AnsiConsole.MarkupLine("[bold underline]🧾 Your final order:[/]");
                                    newOrder.DisplayOrder();

                                    addingDishes = false;
                                    break;

                                default:
                                    AnsiConsole.MarkupLine("[red]Invalid choice. Please try again.[/]");
                                    break;
                            }
                        }
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

                        LogManager.Log($"User '{user.Username}' checked order status.");

                    };
                }

                if (rbacSystem.HasPermission(user, Permission.ChangeOrderStatus))
                {
                    table.AddRow(orderOptionNumber.ToString(), "Change Order Status");
                    orderActions[orderOptionNumber++] = () =>
                    


                    {
                        Console.Clear();

                        if (user.Roles.Contains(UserRole.Chef))
                        {
                            
                            orderManager.DisplayOrdersByStatus(Order.OrderStatus.Placed);
                            if(orderManager.GetOrdersByStatus(Order.OrderStatus.Placed).Count == 0)
                            {
                                return;
                            }
                            int orderId = AnsiConsole.Ask<int>("Enter Order ID to change status: ");
                            var order = orderManager.GetOrderById(orderId);

                            if (order != null && order.Status == Order.OrderStatus.Placed)
                            {
                                order.MarkAsCooked();
                                orderManager.NotifyWaiter(order);
                                LogManager.Log($"Chef '{user.Username}' cooked an order");
                                AnsiConsole.Markup("[green]✅ Order status updated![/]");
                            }
                            else
                            {
                                AnsiConsole.Markup("[red]❌ Order not found.[/]");
                            }
                        }
                        else if (user.Roles.Contains(UserRole.Waiter))
                        {
                            
                            orderManager.DisplayOrdersByStatus(Order.OrderStatus.Cooked);
                            if(orderManager.GetOrdersByStatus(Order.OrderStatus.Cooked).Count == 0)
                            {
                                return;
                            }
                            int orderId = AnsiConsole.Ask<int>("Enter Order ID to change status: ");
                            var order = orderManager.GetOrderById(orderId);

                            if (order != null && order.Status == Order.OrderStatus.Cooked)
                            {
                                order.MarkAsServed();
                                LogManager.Log($"Waiter '{user.Username}' served an order");
                                AnsiConsole.Markup("[green]✅ Order status updated![/]");
                            }
                            else
                            {
                                AnsiConsole.Markup("[red]❌ Order not found.[/]");
                            }
                        }
                        else if (user.Roles.Contains(UserRole.Admin))
                        {
                            // Admin can choose what type of orders to update
                            var statusChoice = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .Title("[bold]Select what to do:[/]")
                                    .HighlightStyle("orange1")
                                    .AddChoices("Cook Placed Orders", "Serve Cooked Orders")
                            );

                            Order.OrderStatus targetStatus = statusChoice == "Cook Placed Orders"
                                ? Order.OrderStatus.Placed
                                : Order.OrderStatus.Cooked;

                            orderManager.DisplayOrdersByStatus(targetStatus);
                            if (orderManager.GetOrdersByStatus(targetStatus).Count == 0)
                            {
                                return;
                            }
                            int orderId = AnsiConsole.Ask<int>("Enter Order ID to update status: ");
                            var order = orderManager.GetOrderById(orderId);

                            if (order != null && order.Status == targetStatus)
                            {
                                if (targetStatus == Order.OrderStatus.Placed)
                                {
                                    order.MarkAsCooked();
                                    orderManager.NotifyWaiter(order);
                                    LogManager.Log($"Admin '{user.Username}' marked order {orderId} as Cooked.");
                                }
                                else if (targetStatus == Order.OrderStatus.Cooked)
                                {
                                    order.MarkAsServed();
                                    LogManager.Log($"Admin '{user.Username}' marked order {orderId} as Served.");
                                }

                                AnsiConsole.Markup("[green]✅ Order status updated![/]");
                            }
                            else
                            {
                                AnsiConsole.Markup("[red]❌ Order not found.[/]");
                            }
                        }
                        else
                        {
                            AnsiConsole.Markup("[red]❌ You do not have permission to change order status.[/]");
                        }
                    }
                    ;
                }

                if (rbacSystem.HasPermission(user, Permission.CheckMyOrders))
                {
                    table.AddRow(orderOptionNumber.ToString(), "Check My Orders");

                    orderActions[orderOptionNumber++] = () => {
                        orderManager.DisplayOrdersByUserId(user.Username); 
                        LogManager.Log($"User '{user.Username}' checked their orders"); 
                    }; 
                }

                if (rbacSystem.HasPermission(user, Permission.PayForOrder))
                {
                    table.AddRow(orderOptionNumber.ToString(), "Pay for Order");
                    orderActions[orderOptionNumber++] = () =>
                    {
                        Console.Clear();
                        orderManager.DisplayOrdersByUserIdAndStatus(user.Username, Order.OrderStatus.Served);
                        if (orderManager.GetOrdersByUserIdAndStatus(user.Username, Order.OrderStatus.Served).Count >0)
                        {
                            int orderId = AnsiConsole.Ask<int>("Enter Order ID to pay: ");
                            var order = orderManager.GetOrderById(orderId);
                            if (order != null)
                            {
                                order.MarkAsPaid();
                                AnsiConsole.Markup("[green]Order paid![/]");
                                LogManager.Log($"User '{user.Username}' paid {order.fullPrice:C} for their order (orderId: {orderId})");
                            }
                            else
                                AnsiConsole.Markup("[red]Order not found or already paid.[/]");
                        }
                        
                    };

          

                }
                table.AddRow(orderOptionNumber.ToString(), "Back to main menu");
                orderActions[orderOptionNumber++] = () =>
                {
                    manageOrders = false;
                    AnsiConsole.MarkupLine("[Orange1]↩️ Returning to main menu...[/]");
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
                table.AddColumn("[Orange1]Option[/]");
                table.AddColumn("[Orange1]Description[/]");
                if (rbacSystem.HasPermission(user, Permission.ViewMenu))
                {
                    Console.Clear();
                    table.AddRow(optionNumber.ToString(), "View a full menu");
                    menuActions[optionNumber++] = () =>
                    {
                        Console.Clear();
                        m1.DisplayMenu();
                    };
                }

                if (rbacSystem.HasPermission(user, Permission.ManageMenu))
                {
                    table.AddRow(optionNumber.ToString(), "Add a dish");
                    menuActions[optionNumber++] = () =>
                    {
                        Console.Clear();
                        AnsiConsole.Markup("[bold Orange1]Enter dish name: [/]");

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
                    AnsiConsole.MarkupLine("[Orange1]↩️ Returning to main menu...[/]");
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
                table.AddColumn("[Orange1]Option[/]");
                table.AddColumn("[Orange1]Description[/]");

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
                    AnsiConsole.MarkupLine($"[Orange1]⚠️ Role {roleToRemove} removed from {username}.[/]");
                };

                // Exit to Main Menu
                table.AddRow(optionNumber.ToString(), "Back to main menu");
                employeeActions[optionNumber++] = () =>
                {
                    manageEmployees = false;
                    AnsiConsole.MarkupLine("[Orange1]↩️ Returning to main menu...[/]");
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
                table.AddColumn("[Orange1]Option[/]");
                table.AddColumn("[Orange1]Description[/]");

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
                    AnsiConsole.MarkupLine("[Orange1]↩️ Returning to main menu...[/]");
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

