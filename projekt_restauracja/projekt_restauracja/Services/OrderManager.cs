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

        public OrderManager()
        {
            orders = new List<Order>();
        }

        public void AddOrder(Order order)
        {
            orders.Add(order);
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

            Console.WriteLine("All orders:");
            foreach (var order in orders)
            {
                order.DisplayOrder();  
            }
        }
    }
}
