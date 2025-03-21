using projekt_restauracja.Models;
using System;
using System.Collections.Generic;
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
            m1.DisplayMenu();
            Console.ReadLine();

            /*
                Console.Write("Enter username: ");
                string username = Console.ReadLine();

                Console.Write("Enter password: ");
                string password = Console.ReadLine();

                User user = AuthenticationService.Login(username, password);

                if (user == null)
                {
                    Console.WriteLine("Invalid login!");
                    return;
                }

                Console.WriteLine($"Welcome, {user.Username} ({user.Role})!");
                ShowMenu(user);
            */
        }
        /*
            static void ShowMenu(User user)
            {
                while (true)
                {
                    Console.WriteLine("\nSelect an option:");

                    if (user.HasPermission("ViewMenu"))
                        Console.WriteLine("1. View Menu");

                    if (user.HasPermission("ManageMenu"))
                        Console.WriteLine("2. Modify Menu");

                    if (user.HasPermission("PlaceOrder"))
                        Console.WriteLine("3. Place Order");

                    if (user.HasPermission("ViewOrders"))
                        Console.WriteLine("4. View Orders");

                    if (user.HasPermission("ChangeOrderStatus"))
                        Console.WriteLine("5. Change Order Status");

                    if (user.HasPermission("ProcessPayments"))
                        Console.WriteLine("6. Process Payments");

                    Console.WriteLine("0. Exit");
                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1" when user.HasPermission("ViewMenu"):
                            Console.WriteLine("Displaying menu...");
                            break;
                        case "2" when user.HasPermission("ManageMenu"):
                            Console.WriteLine("Modifying menu...");
                            break;
                        case "3" when user.HasPermission("PlaceOrder"):
                            Console.WriteLine("Placing order...");
                            break;
                        case "4" when user.HasPermission("ViewOrders"):
                            Console.WriteLine("Viewing orders...");
                            break;
                        case "5" when user.HasPermission("ChangeOrderStatus"):
                            Console.WriteLine("Changing order status...");
                            break;
                        case "6" when user.HasPermission("ProcessPayments"):
                            Console.WriteLine("Processing payment...");
                            break;
                        case "0":
                            return;
                        default:
                            Console.WriteLine("Invalid option!");
                            break;
                    }
                }
            }*/
    }
    
}
