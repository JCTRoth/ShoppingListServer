using ShoppingListServer.Entities;

namespace ShoppingListServer.Models
{
    class Update_Command
    {
        // SyncID of List
        public int SyncID { get; set; }

        public GenericProduct Item { get; set; }
    }
}
