using Spectre.Console;
using System;
using System.Collections.Generic;

namespace projekt_restauracja.Models
{
    internal class Order
    {
        public enum OrderStatus { Placed, Cooked, Served, Paid }

        private static int nextOrderId = 1; // Counter for orders

        private List<Dish> dishes;
        private OrderStatus status;

        public int OrderId { get; private set; }  // Unique order ID
        public string UserId { get; private set; } // User ID

        public IReadOnlyList<Dish> Dishes => dishes;
        public OrderStatus Status => status;

        public Order(string userId)
        {
            OrderId = nextOrderId++;
            dishes = new List<Dish>();
            status = OrderStatus.Placed;
            UserId = userId;
        }

        public void AddDish(Dish dish) => dishes.Add(dish);
        public void RemoveDish(Dish dish) => dishes.Remove(dish);

        public void MarkAsCooked()
        {
            if (status == OrderStatus.Placed)
            {
                status = OrderStatus.Cooked;
                Console.WriteLine("The order has been cooked.");
            }
            else
            {
                Console.WriteLine("Cannot change status to 'Cooked' from the current state.");
            }
        }

        public void MarkAsServed()
        {
            if (status == OrderStatus.Cooked)
            {
                status = OrderStatus.Served;
                Console.WriteLine("The order has been served.");
            }
            else
            {
                Console.WriteLine("Cannot change status to 'Served' from the current state.");
            }
        }

        public void MarkAsPaid()
        {
            if (status == OrderStatus.Served)
            {
                status = OrderStatus.Paid;
                Console.WriteLine("The order has been paid.");
            }
            else
            {
                Console.WriteLine("Cannot change status to 'Paid' from the current state.");
            }
        }

        public void DisplayOrder()
        {
            var table = new Table();
            Console.WriteLine($"Status: {status}");

            table.Border = TableBorder.Rounded;
            table.AddColumn("[bold]Dish[/]");
            table.AddColumn("[bold]Price (PLN)[/]");

            float totalPrice = 0;
            foreach (var dish in dishes)
            {
                table.AddRow(dish.Name, $"{dish.Price:F2}");
                totalPrice += dish.Price;
            }
            table.AddRow(new Markup("[grey]────────────[/]"), new Markup("[grey]────────────[/]"));

            table.AddRow(new Markup("[bold]Total price:[/]"), new Markup($"[green]{totalPrice:F2} PLN[/]"));
            table.AddRow(new Markup("[bold]Status:[/]"), new Markup($"[blue]{status}[/]"));

            AnsiConsole.Render(table);
        }
    }
}
