using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WinRT.Interop;

namespace XCVTransformer.PInvokeAPIs
{
    static class IconAPI
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr LoadImage(IntPtr hinst, string lpszName, uint uType, int cx, int cy, uint fuLoad);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const uint WM_SETICON = 0x0080;
        private const uint ICON_BIG = 1;
        private const uint IMAGE_ICON = 1;
        private const uint LR_LOADFROMFILE = 0x0010;

        private static void SetWindowIcon(string iconPath, IntPtr currentHWND)
        {
            // Obtener el handle de la ventana (HWND)
            IntPtr hwnd = WindowNative.GetWindowHandle(currentHWND);

            // Cargar el icono desde el archivo
            IntPtr hIcon = LoadImage(IntPtr.Zero, iconPath, IMAGE_ICON, 32, 32, LR_LOADFROMFILE);

            // Enviar el mensaje para cambiar el icono de la ventana
            SendMessage(hwnd, WM_SETICON, (IntPtr)ICON_BIG, hIcon);
        }
    }
}
