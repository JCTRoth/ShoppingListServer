using System.Collections.Generic;

namespace ShoppingListServer.Models
{
    public class Category
    {
        // e.g. : "Category":{"Name":"Getr�nke"}
        public string Name { get; set; }

        public string ImagePath { get; set; }

        public List<GenericProduct> ProductList{ get; set; }
    }
}