namespace ShoppingListServer.Entities
{
    // Commands applied on a shoppinglist
    public class Updatelist_Request
    {
        // Add, Remove, Update
        public string Command_Type { get; set; }

        // e.g. new entry when command is add or name of entry when command is remove
        public string Command { get; set; }

    }
}