using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace projekt_restauracja.Models
{
    internal class OrderManager
    {
        private List<Order> orders;

        private float revenue;

        public OrderManager()
        {
            orders = new List<Order>();
        }

        public void AddOrder(Order order)
        {
            orders.Add(order);

            revenue = 0;
            foreach (var item in orders)
            {
                revenue += item.fullPrice;
            }

        }

        public List<Order> GetOrders()
        {
            return orders;
        }


        public void DisplayOrderSummaryByStatus()
        {
            if (orders.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No orders to display.[/]");
                return;
            }

            var paidOrders = orders.Where(o => o.Status == Order.OrderStatus.Paid).ToList();
            var unpaidOrders = orders.Where(o => o.Status != Order.OrderStatus.Paid).ToList();

            AnsiConsole.MarkupLine("\n[bold underline green]✅ Paid Orders:[/]\n");
            if (paidOrders.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]There are no orders with the 'Paid' status.[/]");
            }
            else
            {
                var paidTable = new Table();
                paidTable.Border = TableBorder.Rounded;
                paidTable.AddColumn("[bold]Order ID[/]");
                paidTable.AddColumn("[bold]Customer ID[/]");
                paidTable.AddColumn("[bold]Price (PLN)[/]");

                float paidTotal = 0;

                foreach (var order in paidOrders)
                {
                    paidTable.AddRow(
                        order.OrderId.ToString(),
                        order.UserId,
                        $"[green]{order.fullPrice:F2}[/]"
                    );
                    paidTotal += order.fullPrice;
                }

                paidTable.AddEmptyRow();
                paidTable.AddRow(
                    new IRenderable[]
                    {
                new Markup("[bold yellow]Total:[/]"),
                new Markup(""),
                new Markup($"[bold green]{paidTotal:F2} PLN[/]")
                    }
                );

                AnsiConsole.Write(paidTable);
            }

            AnsiConsole.MarkupLine("\n[bold underline darkorange]🕒 Other Orders:[/]\n");
            if (unpaidOrders.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]No unpaid orders found.[/]");
            }
            else
            {
                var unpaidTable = new Table();
                unpaidTable.Border = TableBorder.Rounded;
                unpaidTable.AddColumn("[bold]Order ID[/]");
                unpaidTable.AddColumn("[bold]Customer ID[/]");
                unpaidTable.AddColumn("[bold]Price (PLN)[/]");
                unpaidTable.AddColumn("[bold]Status[/]");

                foreach (var order in unpaidOrders)
                {
                    unpaidTable.AddRow(
                        order.OrderId.ToString(),
                        order.UserId,
                        $"[orange1]{order.fullPrice:F2}[/]",
                        $"[blue]{order.Status}[/]"
                    );
                }

                AnsiConsole.Write(unpaidTable);
            }
        }

        public Order GetOrderById(int orderId)
        {
            return orders.FirstOrDefault(o => o.OrderId == orderId);
        }
        // Displaying orders for specific users (a customer can display all of their orders(with status))
        public void DisplayOrdersByUserId(string userId)
        {
            var userOrders = orders.Where(o => o.UserId == userId).ToList();

            if (userOrders.Count == 0)
            {
                Console.WriteLine($"No orders found "); 
                return;
            }

            Console.WriteLine($"Orders for user {userId}:");
            foreach (var order in userOrders)
            {
                order.DisplayOrder();  
            }
        }

        /* Displaying orders with a specific status 
         * The chef will be able to display orders with Placed status -> to cook them
         * The waiter will be able to display orders with Cooked status -> to serve them
         */
        public void DisplayOrdersByStatus(Order.OrderStatus status)
        {
            var statusOrders = orders.Where(o => o.Status == status).ToList();

            if (statusOrders.Count == 0)
            {
                Console.WriteLine($"No orders found with status {status}.");
                return;
            }

            Console.WriteLine($"Orders with status {status}:");
            foreach (var order in statusOrders)
            {
                order.DisplayOrder();  
            }
        }

        // Displaying orders with a specific status and UserId - user can see his orders with status eg.served - so he can pay for it 
        public void DisplayOrdersByUserIdAndStatus(string userId, Order.OrderStatus status)
        {
            var filteredOrders = orders
                .Where(o => o.UserId == userId && o.Status == status)
                .ToList();

            if (filteredOrders.Count == 0)
            {
                Console.WriteLine($"No orders found for user {userId} with status {status}.");
                return;
            }

            Console.WriteLine($"Orders for user {userId} with status {status}:");
            foreach (var order in filteredOrders)
            {
                order.DisplayOrder();  
            }
        }

        // Function to display all orders
        public void DisplayAllOrders()
        {
            if (orders.Count == 0)
            {
                Console.WriteLine("No orders.");
                return;
            }
            Console.Clear();
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("🍽️ All Orders 🍽️");

            foreach (var order in orders)
            {
                order.DisplayOrder();  
            }
        }
    }
}
