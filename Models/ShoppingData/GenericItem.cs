namespace ShoppingListServer.Models
{
    public class GenericItem
    {
        // e.g. : {"Name":"Sprite","ImagePath":null,"Category":{"Name":"Getr�nke"}}
        public string Name { get; set; }

        public string ImagePath { get; set; }

        public Category Category { get; set; }
    }
}