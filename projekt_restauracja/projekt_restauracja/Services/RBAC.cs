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
        //menu
        ManageMenu,
        /* only admin -> allows to:
            * (display menu,
            * add dish,
            * modify price)*/
        ViewMenu,
        /* for client -> allows to:
            * DisplayMenu */

        //employees
        ManageEmployees,
        /* only for admin -> allows to:
            * DisplayEmployees
            * AddEmployee
            * FireEmployee (delete)
            * AddRole
            * RemoveRole */

        //clients
        ManageClients,
        /* only for admin -> allows to:
            * AddClient
            * RemoveClient */

        
        //orders
        DisplayOrders, //only for admin (only admin can see all orders)

        CheckOrderStatus, /* chef (sees orders with placed status),
                     * waiter (sees orders with cooked status) */

        CheckMyOrders, //only client - can see orders with his userid

        ChangeOrderStatus, /* chef placed -> cooked
                      * waiter cooked -> served
                      */

        PlaceAnOrder, //only Client  -> placed

        PayForOrder, //only client served -> paid

        //revenues
        ViewRevenue, //only admin

        //logs
        ViewLogs //only admin

    }
    public class RBAC
    {

        private readonly Dictionary<UserRole, List<Permission>> _rolePermissions;


        public RBAC()
        {
            _rolePermissions = new Dictionary<UserRole, List<Permission>>
            {
                { 
                UserRole.Admin, new List<Permission> { 
                    //Menu
                    Permission.ManageMenu, 
                    Permission.ViewMenu, 

                    //employees
                    Permission.ManageEmployees,

                    //clients
                    Permission.ManageClients,

                    //orders
                    Permission.DisplayOrders,
                    Permission.ChangeOrderStatus,
                    Permission.PlaceAnOrder,
                    Permission.PayForOrder,
                      Permission.CheckOrderStatus,

                    //revenue
                    Permission.ViewRevenue,
                 
                    //logs
                    Permission.ViewLogs, 
                  }},
                { 
                UserRole.Customer, new List<Permission> { 
                    Permission.ViewMenu, 
                    Permission.PlaceAnOrder, 
                    Permission.CheckMyOrders, 
                    Permission.PayForOrder } 
                },
                { 
                UserRole.Chef, new List<Permission> { 
                    Permission.CheckOrderStatus, 
                    Permission.ChangeOrderStatus } },
                { 
                UserRole.Waiter, new List<Permission> {
                    Permission.CheckOrderStatus,
                    Permission.ChangeOrderStatus } }
            };
        }

        public bool HasPermission(User user, Permission permission)
        {
            return user.Roles.Any(role => _rolePermissions.ContainsKey(role) && _rolePermissions[role].Contains(permission));
        }


    }
}
