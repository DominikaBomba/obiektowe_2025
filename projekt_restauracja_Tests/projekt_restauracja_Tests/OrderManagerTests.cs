using projekt_restauracja.Models;
using projekt_restauracja;

namespace projekt_restauracja_Tests
{
    public class OrderManagerTests
    {
        private readonly OrderManager _orderManager;
        private readonly User _customer;
        private readonly User _chef;
        private readonly User _waiter;

        public OrderManagerTests()
        {
            _orderManager = new OrderManager();
            _customer = new User("testCustomer", new List<UserRole> { UserRole.Customer });
            _chef = new User("testChef", new List<UserRole> { UserRole.Chef });
            _waiter = new User("testWaiter", new List<UserRole> { UserRole.Waiter });
        }

        [Fact]
        public void AddOrder_ShouldIncreaseOrdersCount()
        {
            // Arrange
            var initialCount = _orderManager.GetOrders().Count;
            var order = new Order(_customer.Username);

            // Act
            _orderManager.AddOrder(order);

            // Assert
            Assert.Equal(initialCount + 1, _orderManager.GetOrders().Count);
        }

        [Fact]
        public void AddOrder_ShouldAddNotificationForChef()
        {
            // Arrange
            var order = new Order(_customer.Username);
            var initialNotifications = _orderManager.GetNotificationsForRole(UserRole.Chef).Count;

            // Act
            _orderManager.AddOrder(order);

            // Assert
            Assert.Equal(initialNotifications + 1, _orderManager.GetNotificationsForRole(UserRole.Chef).Count);
        }

        [Fact]
        public void NotifyWaiter_ShouldAddNotificationForWaiter()
        {
            // Arrange
            var order = new Order(_customer.Username);
            var initialNotifications = _orderManager.GetNotificationsForRole(UserRole.Waiter).Count;

            // Act
            _orderManager.NotifyWaiter(order);

            // Assert
            Assert.Equal(initialNotifications + 1, _orderManager.GetNotificationsForRole(UserRole.Waiter).Count);
        }

        [Fact]
        public void ClearNotifications_ShouldRemoveAllNotificationsForRole()
        {
            // Arrange
            var order = new Order(_customer.Username);
            _orderManager.AddOrder(order); 
            _orderManager.NotifyWaiter(order); 

            // Act
            _orderManager.ClearNotifications(UserRole.Chef);
            _orderManager.ClearNotifications(UserRole.Waiter);

            // Assert
            Assert.Empty(_orderManager.GetNotificationsForRole(UserRole.Chef));
            Assert.Empty(_orderManager.GetNotificationsForRole(UserRole.Waiter));
        }

        [Fact]
        public void GetOrdersByStatus_ShouldReturnOnlyOrdersWithSpecifiedStatus()
        {
            // Arrange
            var order1 = new Order(_customer.Username);
            var order2 = new Order(_customer.Username);
            order2.MarkAsCooked();

            _orderManager.AddOrder(order1);
            _orderManager.AddOrder(order2);

            // Act
            var cookedOrders = _orderManager.GetOrdersByStatus(Order.OrderStatus.Cooked);

            // Assert
            Assert.Single(cookedOrders);
            Assert.Equal(order2.OrderId, cookedOrders[0].OrderId);
        }

        [Fact]
        public void GetOrdersByUserIdAndStatus_ShouldReturnCorrectOrders()
        {
            // Arrange
            var order1 = new Order(_customer.Username);
            var order2 = new Order("anotherCustomer");
            order2.MarkAsCooked();
            var order3 = new Order(_customer.Username);
            order3.MarkAsCooked();

            _orderManager.AddOrder(order1);
            _orderManager.AddOrder(order2);
            _orderManager.AddOrder(order3);

            // Act
            var result = _orderManager.GetOrdersByUserIdAndStatus(_customer.Username, Order.OrderStatus.Cooked);

            // Assert
            Assert.Single(result);
            Assert.Equal(order3.OrderId, result[0].OrderId);
        }

        [Fact]
        public void HasOrdersWithStatus_ShouldReturnTrueWhenOrdersExist()
        {
            // Arrange
            var order = new Order(_customer.Username);
            order.MarkAsCooked();
            _orderManager.AddOrder(order);

            // Act
            var hasCookedOrders = _orderManager.HasOrdersWithStatus(Order.OrderStatus.Cooked);
            var hasPaidOrders = _orderManager.HasOrdersWithStatus(Order.OrderStatus.Paid);

            // Assert
            Assert.True(hasCookedOrders);
            Assert.False(hasPaidOrders);
        }

        [Fact]
        public void GetOrderById_ShouldReturnCorrectOrder()
        {
            // Arrange
            var order = new Order(_customer.Username);
            _orderManager.AddOrder(order);

            // Act
            var foundOrder = _orderManager.GetOrderById(order.OrderId);

            // Assert
            Assert.NotNull(foundOrder);
            Assert.Equal(order.OrderId, foundOrder.OrderId);
        }

        [Fact]
        public void GetOrderById_ShouldReturnNullForNonExistentId()
        {
            // Act
            var foundOrder = _orderManager.GetOrderById(999);

            // Assert
            Assert.Null(foundOrder);
        }
    }
}