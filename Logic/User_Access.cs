using ShoppingListServer.Entities;

namespace ShoppingListServer.Logic
{
    class User_Access
    {
        public static bool Is_User_Allowed_To_Edit(string userID, ShoppingList shoppingList)
        {
            if(shoppingList.OwnerID == userID)
            {
                return true;
            }

            foreach(string id in shoppingList.AcessIDs)
            {
                if (id == userID)
                {
                    return true;
                }
            }

            return false;
        }
    }

}
