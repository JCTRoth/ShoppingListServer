using ShoppingListServer.Models;

namespace ShoppingListServer.Logic
{
    public class User_Access
    {
        public User_Access()
        {

        }

        public bool Is_User_Allowed_To_Edit(string userID, ShoppingList shoppingList)
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

        public bool Is_User_Allowed_To_Delete(string userID, ShoppingList shoppingList)
        {
            // Only Owner Should be able to delete a list
            if (shoppingList.OwnerID == userID)
            {
                return true;
            }

            return false;
        }

    }

}
