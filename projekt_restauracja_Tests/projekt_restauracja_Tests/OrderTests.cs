using projekt_restauracja.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt_restauracja_Tests
{
    public class OrderTests
    {
        [Fact]
        public void AddDish_ShouldIncreaseDishesCountAndTotalPrice()
        {
            // Arrange
            var order = new Order("testUser");
            var dish = new Dish("Test Dish", 10.99f, Category.Main);

            // Act
            order.AddDish(dish);

            // Assert
            Assert.Single(order.Dishes);
            Assert.Equal(10.99f, order.fullPrice);
        }

        [Fact]
        public void RemoveDish_ShouldDecreaseDishesCountAndTotalPrice()
        {
            // Arrange
            var order = new Order("testUser");
            var dish = new Dish("Test Dish", 10.99f, Category.Main);
            order.AddDish(dish);

            // Act
            order.RemoveDish(dish);

            // Assert
            Assert.Empty(order.Dishes);
            Assert.Equal(0f, order.fullPrice);
        }

        [Fact]
        public void MarkAsCooked_ShouldChangeStatusFromPlacedToCooked()
        {
            // Arrange
            var order = new Order("testUser");

            // Act
            order.MarkAsCooked();

            // Assert
            Assert.Equal(Order.OrderStatus.Cooked, order.Status);
        }

        [Fact]
        public void MarkAsCooked_ShouldNotChangeStatusFromNonPlaced()
        {
            // Arrange
            var order = new Order("testUser");
            order.MarkAsCooked(); // Zmień na Cooked

            // Act & Assert
            Assert.Equal(Order.OrderStatus.Cooked, order.Status); // Status nie powinien się zmienić
        }

        [Fact]
        public void MarkAsServed_ShouldChangeStatusFromCookedToServed()
        {
            // Arrange
            var order = new Order("testUser");
            order.MarkAsCooked();

            // Act
            order.MarkAsServed();

            // Assert
            Assert.Equal(Order.OrderStatus.Served, order.Status);
        }

        [Fact]
        public void MarkAsPaid_ShouldChangeStatusFromServedToPaid()
        {
            // Arrange
            var order = new Order("testUser");
            order.MarkAsCooked();
            order.MarkAsServed();

            // Act
            order.MarkAsPaid();

            // Assert
            Assert.Equal(Order.OrderStatus.Paid, order.Status);
        }

        [Fact]
        public void GetOrderName_ShouldReturnCorrectFormat()
        {
            // Arrange
            var order = new Order("testUser");
            order.AddDish(new Dish("Dish1", 10f, Category.Main));
            order.AddDish(new Dish("Dish2", 15f, Category.Dessert));

            // Act
            var orderName = order.GetOrderName();

            // Assert
            Assert.Contains("OrderId:", orderName);
            Assert.Contains("Customer: testUser", orderName);
            Assert.Contains("Dish1", orderName);
            Assert.Contains("Dish2", orderName);
        }
    }
}
