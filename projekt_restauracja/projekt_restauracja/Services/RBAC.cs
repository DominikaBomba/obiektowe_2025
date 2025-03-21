using projekt_restauracja.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt_restauracja.Services
{
    public enum Role
    {
        Admin,
        Customer,
        Chef,
        Waiter
    }
    public enum Permission
    {
        UpdateMenu,
        OrderDish
        ,
        ManageUsers
    }
    internal class RBAC
    {

        private readonly Dictionary<Role, List<Permission>> _rolePermissions;


        public RBAC()
        {
            _rolePermissions = new Dictionary<Role, List<Permission>>
            {
                    { Role.Admin, new List<Permission>{ }},
                    { Role.Customer, new List<Permission>{ } },
                    { Role.Waiter, new List<Permission>{ } }
                };
        }

        public bool HasPermission(User user, Permission permission)
        {
            /*foreach (var role in user.Roles)
            {
                if (_rolePermissions.ContainsKey(role) && _rolePermissions[role].Contains(permission))
                {
                    return true;
                }
            }*/
            return false;
        }


    }
}
