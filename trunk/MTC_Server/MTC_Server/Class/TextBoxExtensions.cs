using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Alta.Class
{
    public static class TextBoxExtensions
    {
        public static bool isEmpty(this TextBox txt)
        {
            if (txt.Text.isEmpty())
                return true;
            return false;
        }

        public static bool isEmail(this TextBox txt)
        {
            if (txt.Text.IsValidEmail())
                return true;
            return false;
        }

        public static bool isIP(this TextBox txt)
        {
            if (txt.Text.isIP())
                return true;
            return false;
        }

        public static bool isPhone(this TextBox txt)
        {
            if (txt.Text.isValidPhone())
            {
                return true;
            }
            return false;
        }
        public static void empty(this TextBox txt)
        {
            txt.Text = string.Empty;
        }
    }
}
