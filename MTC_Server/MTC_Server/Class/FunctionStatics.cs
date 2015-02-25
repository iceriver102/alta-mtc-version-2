using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Alta.Class
{
    public static class FunctionStatics
    {
        public static void closeKeyboard()
        {
#if DEBUG
                uint WM_SYSCOMMAND = 274;
                uint SC_CLOSE = 61536;
                IntPtr KeyboardWnd = FindWindow("IPTip_Main_Window", null);
                PostMessage(KeyboardWnd.ToInt32(), WM_SYSCOMMAND, (int)SC_CLOSE, 0);
#endif
            
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(int hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(String sClassName, String sAppName);

        public static string MD5String(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.ASCII.GetBytes(input));
                String tmp = BitConverter.ToString(data).Replace("-", "");
                return tmp.ToLower();
            }
        }
    }
   
}
