using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XCVTransformer.Helpers
    {
    /*
     * Posiciona la ventana presuntamente cerca del notify area o bandeja del sistema,
     * utiliza un metodo para obtener el RECT de la bandeja del sistema y coloca ahi 
     * la ventana con un ancho alto y offsets personalizables en las constantes.
     * 
     */
    static class SystemTrayPositionHelper
    {
        //Se puede configurar aqui el ancho y alto de la ventana
        const int width = 300;
        const int height = 500;
        //Se puede configurar aqui un ajuste de localizacion de la ventana
        const int hOffset = 150;///+ a la izquierda, - a la derecha
        const int vOffset = -10;///+ hacia arriba, - hacia abajo


        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_SHOWWINDOW = 0x0040;

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        /**
            * Obtiene la posición del ícono de la bandeja del sistema o area de notificaciones, esto lo obtiene en formato RECT, 
            * un struct utilizado para ver el rectangulo de una ventana
            */
        public static RECT GetSystemTrayIconRect(IntPtr hWnd)
        {
            try
            {
                IntPtr systemTrayHandle = FindWindow("Shell_TrayWnd", null);
                IntPtr notificationAreaHandle = FindWindowEx(systemTrayHandle, IntPtr.Zero, "TrayNotifyWnd", null);

                RECT trayIconRect = new RECT();
                GetWindowRect(notificationAreaHandle, out trayIconRect);

                return trayIconRect;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener rectángulo del ícono: {ex.Message}");
            }
            //Aqui estariamos bastante mal, espero no pase, pongo esto para poder luego comprobar si no se consiguio el RECT bien y abortar el posicionamiento para ver al menos la ventana
            return new RECT { left = -1, top = -1, right = -1, bottom = -1};         }

           
        /**
        * Pone la ventana cerca del RECT del notify area
        */
        public static void PositionNearSystemTrayIcon(IntPtr hWnd)
        {
            try
            {
                RECT iconRect = GetSystemTrayIconRect(hWnd);
                //Solo si tenemos RECT valido ejecutamos el posicionamiento
                if(!(iconRect.left == -1 && iconRect.right == -1 && iconRect.top == -1 && iconRect.bottom == -1))
                {
                    int adjustedX = iconRect.right - width - hOffset; 
                    int adjustedY = iconRect.top - height - vOffset;   

                    SetWindowPos(
                        hWnd,
                        IntPtr.Zero,
                        adjustedX,
                        adjustedY,
                        width,
                        height,
                        SWP_NOZORDER | SWP_SHOWWINDOW
                    );
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al posicionar ventana: {ex.Message}");
            }
        }
     }
}
