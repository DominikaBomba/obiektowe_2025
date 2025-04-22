using projekt_restauracja.Models;
using projekt_restauracja.Services;
using projekt_restauracja;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt_restauracja_Tests
{
    public class RBACTests
    {
        private readonly RBAC _rbac;

        public RBACTests()
        {
            _rbac = new RBAC();
        }

        [Fact]
        public void HasPermission_ShouldReturnTrueForAdminPermissions()
        {
            // Arrange
            var admin = new User("admin", new List<UserRole> { UserRole.Admin });

            // Act & Assert
            Assert.True(_rbac.HasPermission(admin, Permission.ManageMenu));
            Assert.True(_rbac.HasPermission(admin, Permission.ViewMenu));
            Assert.True(_rbac.HasPermission(admin, Permission.ManageEmployees));
            Assert.True(_rbac.HasPermission(admin, Permission.DisplayOrders));
        }

        [Fact]
        public void HasPermission_ShouldReturnFalseForUnauthorizedPermissions()
        {
            // Arrange
            var customer = new User("customer", new List<UserRole> { UserRole.Customer });

            // Act & Assert
            Assert.False(_rbac.HasPermission(customer, Permission.ManageMenu));
            Assert.False(_rbac.HasPermission(customer, Permission.ManageEmployees));
            Assert.False(_rbac.HasPermission(customer, Permission.DisplayOrders));
        }

        [Fact]
        public void HasPermission_ShouldWorkWithMultipleRoles()
        {
            // Arrange
            var user = new User("user", new List<UserRole> { UserRole.Customer, UserRole.Waiter });

            // Act & Assert
            Assert.True(_rbac.HasPermission(user, Permission.PlaceAnOrder)); // Customer's permission
            Assert.True(_rbac.HasPermission(user, Permission.ChangeOrderStatus)); // Waiter's permission
            Assert.False(_rbac.HasPermission(user, Permission.ManageMenu)); // Not authorized
        }

        [Fact]
        public void HasPermission_ShouldReturnFalseForUnknownRole()
        {
            // Arrange
            var user = new User("user", new List<UserRole> { (UserRole)999 }); // Nieznana rola

            // Act & Assert
            Assert.False(_rbac.HasPermission(user, Permission.ViewMenu));
        }
    }
}
