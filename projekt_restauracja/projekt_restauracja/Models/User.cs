using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt_restauracja.Models
{
    public enum UserRole
    {
        Admin, Customer, Waiter, Chef
    }

    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public List<UserRole> Roles { get; set; }

        public User(string username, string password)
        {
            Username = username;
            Password = password;
            Roles = new List<UserRole>();
        }

        public bool HasRole(UserRole role)
        {
            return Roles.Contains(role);
        }
    }
}


