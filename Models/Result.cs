namespace ShoppingListServer.Models
{
    // Return Value of API Functions
    // e.g. ShopingList was not found than WasFound == false
    // No need to check on some magical values inside of the empty return object
    public class Result
    {
        public dynamic ReturnValue { get; set; }

        public bool WasFound { get; set; }
    }
}
