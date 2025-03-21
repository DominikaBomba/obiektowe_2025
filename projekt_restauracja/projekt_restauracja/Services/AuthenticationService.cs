using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace projekt_restauracja.Models
{
    public class AuthenticationService
    {
        private Dictionary<string, string> userCredentials = new Dictionary<string, string>();

        public AuthenticationService()
        {
            LoadUsers();
        }

        private void LoadUsers()
        {
            string[] lines = File.ReadAllLines("Data/users.txt");
            foreach (var line in lines.Skip(1))  // Skip header
            {
                var parts = line.Split(',');
                if (parts.Length == 2)
                {
                    userCredentials[parts[0]] = parts[1];  // username -> password
                }
            }
        }

        public User Authenticate(string username, string password)
        {
            if (userCredentials.TryGetValue(username, out string storedPassword) && storedPassword == password)
            {
                return new User(username, password);
            }
            return null;
        }
    }
}
