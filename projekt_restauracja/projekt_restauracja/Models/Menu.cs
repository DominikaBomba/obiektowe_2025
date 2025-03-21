using projekt_restauracja.Models;
using System;
using System.Collections.Generic;
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

        public Menu(string filePath)
        {
            Dishes = new List<Dish>();
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
               
                
                    int price = int.Parse(parts[0]);
                    string category = parts[1];
                    string dishName = parts[2];

                Dishes.Add(new Dish(dishName, price, (Category)Enum.Parse(typeof(Category), category)));

            }
        }

        public void AddDish(string name, int price, Category category)
        {
            if (Dishes.Any(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Danie o takiej nazwie już istnieje.");
                return;
            }

            Dishes.Add(new Dish(name, price, category));
            Console.WriteLine($"Dodano danie: {name}");
        }

        public void RemoveDish(string name)
        {
            var dishToRemove = Dishes.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (dishToRemove != null)
            {
                Dishes.Remove(dishToRemove);
                Console.WriteLine($"Usunięto danie: {name}");
            }
            else
            {
                Console.WriteLine("Nie znaleziono dania do usunięcia.");
            }
        }

        public void ModifyPrice(string name, int newPrice)
        {
            var dishToModify = Dishes.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (dishToModify != null)
            {
                dishToModify.Price = newPrice;
                Console.WriteLine($"Zmieniono cenę dania {name} na {newPrice} zł.");
            }
            else
            {
                Console.WriteLine("Nie znaleziono dania do modyfikacji.");
            }
        }

        public void DisplayMenu()
        {
            foreach(var category in Categories)
            {
                Console.WriteLine($"{category}: ");
                foreach (var dish in Dishes)
                {
                    if(dish.Category == category)
                    {
                        Console.WriteLine($"\t{dish}");

                    }
                    
                }


            }

        }
    }
}
