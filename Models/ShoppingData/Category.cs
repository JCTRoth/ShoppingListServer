using System.Collections.Generic;

namespace ShoppingListServer.Entities
{
    public class Category
    {
        // e.g. : "Category":{"Name":"Getr�nke"}
        public string Name { get; set; }

        public string ImagePath { get; set; }

        public List<Item> ProductList{ get; set; }
    }
}