using ShoppingListServer.Entities;

namespace ShoppingListServer.Models
{
    class Update_Item
    {
        // SyncID of List
        public int SyncID { get; set; }

        public string OldItemName { get; set; }

        public Item NewItem { get; set; }
    }
}
