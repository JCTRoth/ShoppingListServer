using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListServer.Exceptions
{
    public class ShoppingListNotFoundException : Exception
    {
        string ShoppingListId { get; set; }

        public ShoppingListNotFoundException(string _shoppingListId)
        {
            ShoppingListId = _shoppingListId;

            Console.Error.WriteLine("ShoppingListNotFoundException " + _shoppingListId);
        }
    }
}
