﻿namespace ShoppingListServer.Models.Commands
{
    class Remove_Item
    {
        // SyncID of List
        public string ShoppingListId { get; set; }

        public string ItemName { get; set; }

    }
}
