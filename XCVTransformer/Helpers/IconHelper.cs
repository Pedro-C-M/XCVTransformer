using System;
using System.Runtime.InteropServices;

namespace XCVTransformer.Helpers
{
    /*
     * Permite cambiar el icono de una ventana.
     * 
     * Hace uso de la API de Windows user32.dll de la cual utiliza los métodos:
     * 
     *  - LoadImage: Carga una imagen desde un archivo
     *  
     *  - SendMessage: Envía un mensaje a una ventana para modificar su apariencia
     */
    static class IconHelper
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr LoadImage(IntPtr hinst, string lpszName, uint uType, int cx, int cy, uint fuLoad);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const uint WM_SETICON = 0x0080;
        private const uint ICON_BIG = 1;
        private const uint IMAGE_ICON = 1;
        private const uint LR_LOADFROMFILE = 0x0010;

        /**
         * Cambia el icono de la ventana al deseado
         * 
         * iconPath: Dirección de la imagen a cargar
         * hwmd: Handler de la ventana donde queremos el icono
         */
        public static void SetWindowIcon(string iconPath, IntPtr hwnd)
        {
            // Cargar el icono desde el archivo
            IntPtr hIcon = LoadImage(IntPtr.Zero, iconPath, IMAGE_ICON, 32, 32, LR_LOADFROMFILE);

            // Enviar el mensaje para cambiar el icono de la ventana
            SendMessage(hwnd, WM_SETICON, (IntPtr)ICON_BIG, hIcon);
        }
    }
}
