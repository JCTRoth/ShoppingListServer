using System;

namespace ShoppingListServer
{
    public class Tools
    {
        // Returns false if empty
        public static bool Eval_String_Boolean(string value)
        {
            if (False_If_Empty_Or_Null(value))
                return Boolean.Parse(value);
            else
                return false;
        }

        public static bool False_If_Empty_Or_Null(string value)
        {
            if (value != "" && value != null)
                return true;
            else
                return false;
        }

        public static bool Is_Valid_Email(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
