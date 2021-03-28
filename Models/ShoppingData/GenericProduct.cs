using Newtonsoft.Json;
using System.Collections.Generic;

namespace ShoppingListServer.Entities
{
    public class GenericProduct
    {
        // e.g. : {"Item":{"Name":"Sprite","ImagePath":null,"Category":{"Name":"Getränke"}},"Count":9,"Checked":false}
        public GenericItem Item { get; set; }

        public int Count { get; set; }

        public bool Checked { get; set; }
    }
}