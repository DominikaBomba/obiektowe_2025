using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using projekt_restauracja.Services;

namespace projekt_restauracja.Models
{
    /*
    public class AuthenticationService
    {
        private List<User> users = new List<User>(); // List of Users

        public AuthenticationService()
        {
            LoadUsers();
        }

        // Load Users without storing passwords in plain text
        private void LoadUsers()
        {
            string[] lines = File.ReadAllLines("userPasswords.txt");
            foreach (var line in lines.Skip(1))  // Skip header if present
            {
                var parts = line.Split(',');
                if (parts.Length >= 1)
                {
                    users.Add(new User(parts[0], "")); // Password is not stored here
                }
            }
        }

        public User Authenticate(string username, string password)
        {
            if (PasswordManager.VerifyPassword(username, password))
            {
                return users.FirstOrDefault(u => u.Username == username);
            }
            return null;
        }
    }*/
}
