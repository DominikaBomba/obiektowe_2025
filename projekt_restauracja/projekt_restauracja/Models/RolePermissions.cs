using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt_restauracja.Models
{
    public static class RolePermissions
    {
        private static readonly Dictionary<UserRole, List<string>> permissions = new Dictionary<UserRole, List<string>>
    {
        { UserRole.Admin, new List<string> { "ManageMenu", "ViewMenu", "PlaceOrder", "ChangeOrderStatus", "ViewLogs", "ProcessPayments" } },
        { UserRole.Customer, new List<string> { "ViewMenu", "PlaceOrder", "CheckOrderStatus", "ProcessPayments" } },
        { UserRole.Chef, new List<string> { "ViewOrders", "ChangeOrderStatus" } },
        { UserRole.Waiter, new List<string> { "ViewOrders", "ServeOrder" } }
    };

        public static bool CanPerform(UserRole role, string action)
        {
            return permissions.ContainsKey(role) && permissions[role].Contains(action);
        }
    }
}
