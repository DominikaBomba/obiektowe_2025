using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;
using projekt_restauracja.Models;

namespace projekt_restauracja.Models
{
   

    public class User
    {
        public string Username { get; set; }
       
        public List<UserRole> Roles { get; set; }
        

        public User(string username, List<UserRole> roles)
        {
            Username = username;
        
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
    public class Chef : User
    {
        public Chef(string username, List<UserRole> roles) : base(username, roles)
        {
        }
        public void ShowNotifications(List<string> notifications)
        {
            if (notifications.Count > 0)
            {
                var lines = new List<string> { "🔔 New Orders to Cook:" };
                lines.AddRange(notifications.Select(note => $"• {note}"));

                var panel = new Panel(new Text(string.Join("\n", lines)))
                    .Border(BoxBorder.Rounded)
                    .Header("Chef Notifications", Justify.Center)
                    .Padding(new Padding(1, 1, 1, 1))
                    .Expand();

                AnsiConsole.Render(panel);
            }
        }
    }
    public class Waiter : User
    {
        public Waiter(string username, List<UserRole> roles) : base(username, roles)
        {

        }

        public void ShowNotifications(List<string> notifications)
        {
            if (notifications.Count > 0)
            {
                var lines = new List<string> { "🔔 New Orders to Serve:" };
                lines.AddRange(notifications.Select(note => $"• {note}"));

                var panel = new Panel(new Text(string.Join("\n", lines)))
                    .Border(BoxBorder.Rounded)
                    .Header("Waiter Notifications", Justify.Center)
                    .Padding(new Padding(1, 1, 1, 1))
                    .Expand();

                AnsiConsole.Render(panel);
            }
        }
    }


}


