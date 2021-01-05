using System.Collections.Generic;

namespace ShoppingListServer.Entities
{
    public class Category
    {
        // e.g. : "Category":{"Name":"Getränke"}
        public string Name { get; set; }

        public string ImagePath { get; set; }

        public List<Item> ProductList{ get; set; }
    }
}