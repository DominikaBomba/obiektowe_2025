using projekt_restauracja.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt_restauracja.Services
{
    public enum Permission
    {
        ManageMenu,
        ViewMenu,
        PlaceOrder,
        ChangeOrderStatus,
        ViewLogs,
        ProcessPayments,
        ViewOrders,
        ServeOrder,
        CheckOrderStatus
    }
    internal class RBAC
    {

        private readonly Dictionary<UserRole, List<Permission>> _rolePermissions;


        public RBAC()
        {
            _rolePermissions = new Dictionary<UserRole, List<Permission>>
            {
                { 
                UserRole.Admin, new List<Permission> { 
                    Permission.ManageMenu, 
                    Permission.ViewMenu, 
                    Permission.PlaceOrder,
                    Permission.ChangeOrderStatus, 
                    Permission.ViewLogs, 
                    Permission.ProcessPayments,
                    Permission.ViewOrders,
                    Permission.ServeOrder,
                    Permission.CheckOrderStatus}},
                { 
                UserRole.Customer, new List<Permission> { 
                    Permission.ViewMenu, 
                    Permission.PlaceOrder, 
                    Permission.CheckOrderStatus, 
                    Permission.ProcessPayments } },
                { 
                UserRole.Chef, new List<Permission> { 
                    Permission.ViewOrders, 
                    Permission.ChangeOrderStatus } },
                { 
                UserRole.Waiter, new List<Permission> { 
                    Permission.ViewOrders, 
                    Permission.ServeOrder } }
            };
        }

        public bool HasPermission(User user, Permission permission)
        {
            return user.Roles.Any(role => _rolePermissions.ContainsKey(role) && _rolePermissions[role].Contains(permission));
        }


    }
}
