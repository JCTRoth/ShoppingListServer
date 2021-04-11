using ShoppingListServer.Entities;

namespace ShoppingListServer.Models
{
    class Update_Item
    {
        // SyncID of List
        public string ShoppingListId { get; set; }

        public string OldItemName { get; set; }

        public GenericProduct NewItem { get; set; }
    }
}
