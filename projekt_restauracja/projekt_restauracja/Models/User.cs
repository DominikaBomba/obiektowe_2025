using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt_restauracja.Models
{
   

    public class User
    {
        public string Username { get; set; }
        //public string Password { get; set; }
        public List<UserRole> Roles { get; set; }

        public User(string username, List<UserRole> roles)
        {
            Username = username;
            //Password = password;
            Roles = roles;
        }

        public bool HasRole(UserRole role)
        {
            return Roles.Contains(role);
        }

        public void AddRole(UserRole role)
        {
            if (!Roles.Contains(role))
            {
                Roles.Add(role);
            }
        }
    }
}


