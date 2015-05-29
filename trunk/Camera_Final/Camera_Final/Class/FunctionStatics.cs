using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Alta.Class
{
    public static class FunctionStatics
    {
        public static byte[] toByteArray(this Object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                byte[] retult= ms.ToArray();
                ms.Flush();
                return retult;
            }
        }
        public static Object toObject(this byte[] bytes)
        {
            if (bytes == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(bytes))
            {
               return bf.Deserialize(ms);
            }
        }
        public static string getCPUID()
        {
            String cpuid = "";
            try
            {
                ManagementObjectSearcher mbs = new ManagementObjectSearcher("Select ProcessorID From Win32_processor");
                ManagementObjectCollection mbsList = mbs.Get();

                foreach (ManagementObject mo in mbsList)
                {
                    cpuid = mo["ProcessorID"].ToString();
                }
                return cpuid;
            }
            catch (Exception) { return cpuid; }
        }
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

       
    }

}
