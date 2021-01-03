using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListServer
{
    public class Helper
    {
        // Returns false if empty
        public static bool Eval_String_Boolean(string value)
        {
            if (value != "" || value != null)
                return Boolean.Parse(value);
            else
                return false;
        }

        public static bool False_If_Empty_Or_Null(string value)
        {
            if (value != "" || value != null)
                return true;
            else
                return false;
        }
    }
}
