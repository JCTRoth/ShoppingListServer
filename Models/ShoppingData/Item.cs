using Newtonsoft.Json;
using System.Collections.Generic;

namespace ShoppingListServer.Entities
{
    public class Item
    {
        // e.g. : {"Item":{"Name":"Sprite","ImagePath":null,"Category":{"Name":"Getränke"}},"Count":9,"Checked":false}
        
        [JsonProperty(PropertyName = "Item")]
        public GenericItem GenericItem { get; set; }

        public int Count { get; set; }

        public bool Checked { get; set; }
    }
}