using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinRT.Interop;

namespace XCVTransformer.Helpers
{
    public class TrayHelper
    {
        // Estructura de la notificación para el ícono de la bandeja
        [StructLayout(LayoutKind.Sequential)]
        public struct NOTIFYICONDATA
        {
            public uint cbSize;
            public IntPtr hwnd;
            public uint uID;
            public uint uFlags;
            public uint uCallbackMessage;
            public IntPtr hIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szTip;
        }

        // Constantes para las banderas
        private const uint NIF_MESSAGE = 0x00000001;
        private const uint NIF_ICON = 0x00000002;
        private const uint NIF_TIP = 0x00000004;

        private const uint WM_MOUSEMOVE = 0x2000;
        private const uint WM_LBUTTONUP = 0x0202;

        private const uint IMAGE_ICON = 1;
        private const uint LR_LOADFROMFILE = 0x0010;



        // Importar las funciones necesarias desde las bibliotecas de Windows
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr LoadIcon(IntPtr hInstance, string lpIconName);

        [DllImport("shell32.dll", SetLastError = true)]
        public static extern IntPtr Shell_NotifyIcon(uint dwMessage, ref NOTIFYICONDATA lpData);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr LoadImage(IntPtr hinst, string lpszName, uint uType, int cx, int cy, uint fuLoad);

        public static void ShowTrayIcon(Window window, string tooltip, string iconPath)
        {
            // Crear una estructura para la notificación
            NOTIFYICONDATA nid = new NOTIFYICONDATA();
            nid.cbSize = (uint)Marshal.SizeOf(typeof(NOTIFYICONDATA));
            nid.hwnd = WindowNative.GetWindowHandle(window); // Usamos GetWindowHandle para obtener el handle
            nid.uID = 1;
            nid.uFlags = NIF_ICON | NIF_TIP;
            nid.uCallbackMessage = WM_MOUSEMOVE;
            nid.hIcon = LoadImage(IntPtr.Zero, iconPath, IMAGE_ICON, 32, 32, LR_LOADFROMFILE);
            //nid.hIcon = LoadIcon(IntPtr.Zero, iconPath); // Asegúrate de tener una ruta de ícono válida
            if (nid.hIcon == IntPtr.Zero)
            {
                Debug.WriteLine($"Error al cargar el ícono desde la ruta: {iconPath}");
            }
            else
            {
                Debug.WriteLine($"Va guchi el icono");
            }
            nid.szTip = tooltip;

            // Agregar el ícono a la bandeja del sistema
            Shell_NotifyIcon(0x00000000, ref nid); // Añadir el ícono a la bandeja
        }

        public static void RemoveTrayIcon(Window window)
        {
            NOTIFYICONDATA nid = new NOTIFYICONDATA();
            nid.cbSize = (uint)Marshal.SizeOf(typeof(NOTIFYICONDATA));
            nid.hwnd = WindowNative.GetWindowHandle(window); // Usamos GetWindowHandle para obtener el handle
            nid.uID = 1;

            // Eliminar el ícono de la bandeja
            Shell_NotifyIcon(0x00000002, ref nid); // Eliminar el ícono de la bandeja
        }
    }
}