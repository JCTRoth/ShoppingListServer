using System.Collections.Generic;

namespace ShoppingListServer.Entities
{
    public class GenericItem
    {
        // e.g. : {"Name":"Sprite","ImagePath":null,"Category":{"Name":"Getr�nke"}}
        public string Name { get; set; }

        public string ImagePath { get; set; }

        public Category Category { get; set; }
    }
}