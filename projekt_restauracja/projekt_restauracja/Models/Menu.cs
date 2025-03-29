using projekt_restauracja.Models;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt_restauracja.Models
{
    public enum Category
    {
        Appetizer, //przekąski/startery
        Main, //danie główne
        Dessert, //desery
        Beverage //napoje
    }
    internal class Menu
    {
        public List<Dish> Dishes { get; private set; }
        public List<Category> Categories { get; private set; }

        private string filePath;

        public Menu(string filePath)
        {
            Dishes = new List<Dish>();
            this.filePath = filePath;
            LoadDishesFromFile(filePath);
            Categories = new List<Category>
            {
                Category.Appetizer, //przekąski/startery
                Category.Main, //danie główne
                Category.Dessert, //desery
                Category.Beverage //napoje
            };
        }

        private void LoadDishesFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Plik nie istnieje.");
                return;
            }

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var parts = line.Split(',');

                float price = float.Parse(parts[0], CultureInfo.InvariantCulture.NumberFormat);
                string category = parts[1];
                string dishName = parts[2];

                Dishes.Add(new Dish(dishName, price, (Category)Enum.Parse(typeof(Category), category)));
            }
        }

        private void SaveDishesToFile()
        {
            var lines = Dishes.Select(d => $"{d.Price.ToString(CultureInfo.InvariantCulture)},{d.Category},{d.Name}").ToArray();
            File.WriteAllLines(filePath, lines);
        }

        public void AddDish(string name, float price, Category category)
        {
            if (Dishes.Any(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Danie o takiej nazwie już istnieje.");
                return;
            }

            Dishes.Add(new Dish(name, price, category));
            Console.WriteLine($"Dodano danie: {name}");
            SaveDishesToFile();  // Zapisz zmiany w pliku
        }

        public void RemoveDish(string name)
        {
            var dishToRemove = Dishes.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (dishToRemove != null)
            {
                Dishes.Remove(dishToRemove);
                Console.WriteLine($"Usunięto danie: {name}");
                SaveDishesToFile();  // Zapisz zmiany w pliku
            }
            else
            {
                Console.WriteLine("Nie znaleziono dania do usunięcia.");
            }
        }

        public void ModifyPrice(string name, float newPrice)
        {
            var dishToModify = Dishes.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (dishToModify != null)
            {
                dishToModify.Price = newPrice;
                Console.WriteLine($"Zmieniono cenę dania {name} na {newPrice} zł.");
                SaveDishesToFile();  // Zapisz zmiany w pliku
            }
            else
            {
                Console.WriteLine("Nie znaleziono dania do modyfikacji.");
            }
        }

        public void DisplayMenu()
        {
            var table = new Table();
            table.Border = TableBorder.Rounded;
            table.AddColumn("[cyan]Category[/]");
            table.AddColumn("[green]Dish[/]");
            table.AddColumn("[yellow]Price (PLN)[/]");

            int index_category = 1;
            int index_dish = 1;
            foreach (var category in Categories)
            {
                // Dodanie kategorii z emoji
                string categoryEmoji = string.Empty;
                switch (category)
                {
                    case Category.Appetizer:
                        categoryEmoji = "🥗";
                        break;
                    case Category.Main:
                        categoryEmoji = "🍽️";
                        break;
                    case Category.Dessert:
                        categoryEmoji = "🍰";
                        break;
                    case Category.Beverage:
                        categoryEmoji = "🥤";
                        break;
                    default:
                        categoryEmoji = "🍴";
                        break;
                }

                // Dodanie kategorii do tabeli
                table.AddRow($"[bold]{index_category}. {categoryEmoji} {category}[/]", "", "");

                foreach (var dish in Dishes)
                {
                    if (dish.Category == category)
                    {
                        // Dodanie dania z ceną
                        table.AddRow("", $"{dish.Name}", $"{dish.Price:C}");
                        index_dish++;
                    }
                }
                index_category++;
            }

            AnsiConsole.Render(table);
        }



    }
}
