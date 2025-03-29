using Spectre.Console;
using System;
using System.Collections.Generic;

namespace projekt_restauracja.Models
{

    public class Order
    {
        public enum OrderStatus { Placed, Cooked, Served, Paid }
        public float fullPrice { get; private set; }

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

        public void AddDish(Dish dish)
        {
            if (status == OrderStatus.Placed)
            {
                dishes.Add(dish);

                fullPrice += dish.Price;
            }

        }
        public void RemoveDish(Dish dish)
        {
            if (status == OrderStatus.Placed)
            {
                dishes.Remove(dish);
                fullPrice -= dish.Price;
            }
        }

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
            table.Border = TableBorder.Rounded;

            table.AddColumn(new TableColumn("[bold]Dish[/]"));
            table.AddColumn(new TableColumn("[bold]Price (PLN)[/]"));

            foreach (var dish in dishes)
            {
                table.AddRow(dish.Name, $"{dish.Price:F2}");
            }

            table.AddRow(new Markup("[grey]────────────[/]"), new Markup("[grey]────────────[/]"));

            table.AddRow(new Markup("[bold]Total price:[/]"), new Markup($"[green]{fullPrice:F2} PLN[/]"));
            table.AddRow(new Markup("[bold]Status:[/]"), new Markup($"[blue]{status}[/]"));
            table.AddRow(new Markup("[grey]────────────[/]"), new Markup("[grey]────────────[/]"));

            table.AddRow(new Markup($"[bold yellow]Order ID: [/]"), new Markup($"[bold yellow] {OrderId}[/]"));


            AnsiConsole.Render(table);
        }
    }
}
