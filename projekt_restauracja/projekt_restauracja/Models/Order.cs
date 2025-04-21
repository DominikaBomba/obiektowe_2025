using Spectre.Console;
using System;
using System.Collections.Generic;

namespace projekt_restauracja.Models
{
    public interface IOrder
    {
        int OrderId { get; }
        string UserId { get; }
        IReadOnlyList<Dish> Dishes { get; }
     

        void AddDish(Dish dish);
        void RemoveDish(Dish dish);
        void MarkAsCooked();
        void MarkAsServed();
        void MarkAsPaid();
        void DisplayOrder();
        string GetOrderName();
    }

    public class Order : IOrder
    {
        public DateTime StatusChangedTime { get; private set; }

        public enum OrderStatus { Placed, Cooked, Served, Paid }
        public float fullPrice { get; private set; }

        private static int nextOrderId = 1;
        private List<Dish> dishes;
        private OrderStatus status;

        public int OrderId { get; private set; }
        public string UserId { get; private set; }

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
        public string GetOrderName()
        {
            string orderDetails = $"OrderId: {this.OrderId}, Customer: {this.UserId} [";
            foreach (var dish in dishes)
            {
                orderDetails += $"{dish.Name}, ";
            }
            orderDetails = orderDetails.TrimEnd(',', ' ');
            orderDetails += "]";
            return orderDetails;
        }



        private void ChangeStatus(OrderStatus newStatus, string message)
        {
            status = newStatus;
            
            AnsiConsole.MarkupLine($"[green]{message}[/]");
           
        }

        public void MarkAsCooked()
        {
            if (status == OrderStatus.Placed)
                ChangeStatus(OrderStatus.Cooked, "The order has been cooked.");
            else
                AnsiConsole.MarkupLine("[red]Cannot change status to 'Cooked' from the current state.[/]");
        }

        public void MarkAsServed()
        {
            if (status == OrderStatus.Cooked)
                ChangeStatus(OrderStatus.Served, "The order has been served.");
            else
                AnsiConsole.MarkupLine("[red]Cannot change status to 'Served' from the current state.[/]");
        }

        public void MarkAsPaid()
        {
            if (status == OrderStatus.Served)
                ChangeStatus(OrderStatus.Paid, "The order has been paid.");
            else
                AnsiConsole.MarkupLine("[red]Cannot change status to 'Paid' from the current state.[/]");
        }


        public void DisplayOrder()
        {
            var table = new Table();
            table.Border = TableBorder.Rounded;

            table.AddColumn(new TableColumn("[orange1]Dish[/]"));
            table.AddColumn(new TableColumn("[orange1]Price (PLN)[/]"));

            foreach (var dish in dishes)
            {
                table.AddRow(new Markup($"[white]{dish.Name}[/]"), new Markup($"[white]{dish.Price:F2}[/]"));
            }

            table.AddRow(new Markup("────────────"), new Markup("────────────"));

            table.AddRow(new Markup("[grey]Order ID:[/]"), new Markup($"[grey]{OrderId}[/]"));
            table.AddRow(new Markup("[grey]Status:[/]"), new Markup($"[grey]{status}[/]"));

            table.AddRow(new Markup("────────────"), new Markup("────────────"));

            table.AddRow(new Markup("[orange1]Total price:[/]"), new Markup($"[orange1]{fullPrice:F2} PLN[/]"));

            AnsiConsole.Render(table);
        }

    }

}
