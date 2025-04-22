using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Spectre.Console;

namespace projekt_restauracja.Services
{
    public class PasswordManager
    {

        public enum OrderStatus { Placed, Cooked, Served, Paid }

        private static readonly string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            private static readonly string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.FullName;

            private static readonly string _passwordFilePath = Path.Combine(projectDirectory, "Data", "userPasswords.txt");
            private static readonly string _rolesFilePath = Path.Combine(projectDirectory, "Data", "userRoles.txt");

            public static event Action<string, bool> PasswordVerified;

        public static string GetRoleFilePath()
        {
            return _rolesFilePath;
        }

        static PasswordManager()
        {
            if (!File.Exists(_passwordFilePath))
            {
                File.Create(_passwordFilePath).Dispose();
            }
            if (!File.Exists(_rolesFilePath))
            {
                File.Create(_rolesFilePath).Dispose();
            }
        }

        

        public static bool VerifyPassword(string username, string password)
        {
            string hashedPassword = HashPassword(password);
            foreach (var line in File.ReadLines(_passwordFilePath))
            {
                var parts = line.Split(',');
                if (parts[0] == username && parts[1] == hashedPassword)
                {
                    PasswordVerified?.Invoke(username, true);
                    return true;
                }
            }
            PasswordVerified?.Invoke(username, false);
            return false;
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        public static void AddUser(string username, string password, UserRole role)
        {
          
            if (File.ReadLines(_passwordFilePath).Any(line => line.Split(',')[0] == username))
            {
                Console.WriteLine($"User {username} already exists.");
                return;
            }

            // haslo
            string hashedPassword = HashPassword(password);
            File.AppendAllText(_passwordFilePath, $"{username},{hashedPassword}\n");

            // rola
            
            if (!File.Exists(_rolesFilePath))
                File.Create(_rolesFilePath).Dispose();
            
            File.AppendAllText(_rolesFilePath, $"{username},{role}\n");

            Console.WriteLine($"User {username} added with role {role}.");
        }
        public static List<UserRole> GetUserRoles(string username)
        {
            if (!File.Exists(_rolesFilePath))
                return new List<UserRole>();

            foreach (var line in File.ReadLines(_rolesFilePath))
            {
                var parts = line.Split(',');
                if (parts[0] == username)
                {
                    
                    List<string> roleNames = new List<string>();
                    for (int i = 1; i < parts.Length; i++)
                    {
                        roleNames.Add(parts[i]);
                    }

                   
                    List<UserRole> userRoles = new List<UserRole>();
                    foreach (var role in roleNames)
                    {
                        if (Enum.TryParse(role, out UserRole parsedRole))
                        {
                            userRoles.Add(parsedRole);
                        }
                    }

                    return userRoles;
                }
            }
            return new List<UserRole>();
        }
        public static void AddUserRole(string username, UserRole newRole)
        {
            // Ensure the file exists before attempting to read/write
           /* if (!File.Exists(_rolesFilePath))
                File.Create(_rolesFilePath).Close();*/

            List<string> lines = File.ReadAllLines(_rolesFilePath).ToList();
            bool userFound = false;

            for (int i = 0; i < lines.Count; i++)
            {
                var parts = lines[i].Split(',');

                if (parts[0] == username)
                {
                    List<string> roles = new List<string>(parts.Skip(1));

                    // Check if the role already exists
                    if (!roles.Contains(newRole.ToString()))
                    {
                        roles.Add(newRole.ToString());
                        lines[i] = username + "," + string.Join(",", roles);
                        File.WriteAllLines(_rolesFilePath, lines);
                    }
                    return;
                }
            }

            // If the user is not found, add a new entry
            if (!userFound)
            {
                File.AppendAllText(_rolesFilePath, $"{username},{newRole}{Environment.NewLine}");
            }
        }
        public static void RemoveUserRole(string username, UserRole roleToRemove)
        {
            // Ensure the file exists before attempting to read/write
            if (!File.Exists(_rolesFilePath))
                return;

            List<string> lines = File.ReadAllLines(_rolesFilePath).ToList();
            bool userFound = false;

            for (int i = 0; i < lines.Count; i++)
            {
                var parts = lines[i].Split(',');

                if (parts[0] == username)
                {
                    userFound = true;
                    List<string> roles = new List<string>(parts.Skip(1));

                    // Remove the role if it exists
                    if (roles.Contains(roleToRemove.ToString()))
                    {
                        roles.Remove(roleToRemove.ToString());

                        // If no roles are left, remove the whole line, else update the entry
                        if (roles.Count == 0)
                            lines.RemoveAt(i);
                        else
                            lines[i] = username + "," + string.Join(",", roles);

                        File.WriteAllLines(_rolesFilePath, lines);
                    }
                    return;
                }
            }
        }

        public static void DisplayAllEmployees()
        {
            if (!File.Exists(_rolesFilePath))
            {
                AnsiConsole.MarkupLine("[red]No employees found![/]");
                return;
            }

            var lines = File.ReadAllLines(_rolesFilePath);
            bool hasEmployees = false;

            var table = new Table();
            table.Border = TableBorder.Rounded;
            table.AddColumn("[cyan]Username[/]");
            table.AddColumn("[green]Roles[/]");

            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length < 2) continue;  // Skip invalid lines

                string username = parts[0];
                List<UserRole> roles = parts.Skip(1)
                                            .Where(role => Enum.TryParse(role, out UserRole _))
                                            .Select(role => (UserRole)Enum.Parse(typeof(UserRole), role))
                                            .ToList();

                // Only display users with Chef or Waiter role
                if (roles.Contains(UserRole.Chef) || roles.Contains(UserRole.Waiter))
                {
                    hasEmployees = true;
                    table.AddRow($"[yellow]{username}[/]", $"[green]{string.Join(", ", roles)}[/]");
                }
            }

            if (!hasEmployees)
            {
                AnsiConsole.MarkupLine("[red]No employees found![/]");
                return;
            }

            AnsiConsole.Render(table);
        }

        public static void DisplayCustomers()
        {
            if (!File.Exists(GetRoleFilePath()))
            {
                AnsiConsole.MarkupLine("[red]No customers found![/]");
                return;
            }

            var lines = File.ReadAllLines(GetRoleFilePath());
            bool hasCustomers = false;

            var table = new Table();
            table.Border = TableBorder.Rounded;
            table.AddColumn("[cyan]Username[/]");
            table.AddColumn("[green]Roles[/]");

            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length < 2) continue;  // Skip invalid lines

                string username = parts[0];
                List<UserRole> roles = parts.Skip(1)
                                             .Where(role => Enum.TryParse(role, out UserRole _))
                                             .Select(role => (UserRole)Enum.Parse(typeof(UserRole), role))
                                             .ToList();

                // Only display users with Customer role
                if (roles.Contains(UserRole.Customer))
                {
                    hasCustomers = true;
                    table.AddRow($"[yellow]{username}[/]", $"[green]{string.Join(", ", roles)}[/]");
                }
            }

            if (!hasCustomers)
            {
                AnsiConsole.MarkupLine("[red]No customers found![/]");
                return;
            }

            AnsiConsole.Render(table);
        }
    }
}
