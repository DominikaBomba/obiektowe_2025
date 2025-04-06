using System;

namespace projekt_restauracja.Models
{
    internal class Dish
    {
        public string Name { get; set; }
        public float Price { get; set; }
        public Category Category { get; set; }

        public Dish(string name, float price, Category category)
        {
            Name = name;
            Price = price;
            Category = category;
        }

        public override string ToString()
        {
            return $"{Name} - {Price:F2} zł ";
        }
    }
}
