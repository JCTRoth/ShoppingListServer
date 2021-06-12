namespace ShoppingListServer.Models.Commands
{
    class Update_Item
    {
        // SyncID of List
        public string ShoppingListId { get; set; }

        public string OldItemName { get; set; }

        public GenericItem NewItem { get; set; }
    }
}
