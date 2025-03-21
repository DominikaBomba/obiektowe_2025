using System;

namespace projekt_restauracja.Models
{
    internal class Dish
    {
        public string Name { get; set; }
       public int  Price { get; set; }
        public Category Category { get; set; }

        public Dish(string name, int price, Category category)
        {
            Name = name;
            Price = price;
            Category = category;
        }

        public override string ToString()
        {
            return $"{Name} - {Price} zł ";
        }
    }
}
