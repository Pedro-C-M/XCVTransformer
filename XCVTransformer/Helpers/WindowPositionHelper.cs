using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
    static class SystemTrayPositionHelper
        {
            // Constantes y estructuras
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct NOTIFYICONIDENTIFIER
            {
                public uint cbSize;
                public IntPtr hWnd;
                public uint uID;
                public Guid guidItem;
            }

            // Constantes para SetWindowPos
            private const uint SWP_NOZORDER = 0x0004;
            private const uint SWP_SHOWWINDOW = 0x0040;

            // Declaraciones de PInvoke
            [DllImport("shell32.dll")]
            private static extern int Shell_NotifyIconGetRect(ref NOTIFYICONIDENTIFIER identifier, out RECT iconLocation);

            [DllImport("user32.dll")]
            private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

            /// <summary>
            /// Obtiene la posición del ícono de la bandeja del sistema
            /// </summary>
            /// <param name="hWnd">Handle de la ventana asociada al ícono</param>
            /// <returns>Rectángulo con la posición del ícono</returns>
            public static RECT GetSystemTrayIconRect(IntPtr hWnd)
            {
                var notifyIconData = new NOTIFYICONIDENTIFIER
                {
                    cbSize = (uint)Marshal.SizeOf<NOTIFYICONIDENTIFIER>(),
                    hWnd = hWnd,  // Especificar el handle de la ventana
                    uID = 1,      // ID único para el ícono (generalmente 1 para el primer ícono)
                    guidItem = Guid.Empty
                };

                RECT iconRect = new RECT();

                try
                {
                    // Intentar obtener el rectángulo del ícono de la bandeja
                    int result = Shell_NotifyIconGetRect(ref notifyIconData, out iconRect);

                    if (result != 0)
                    {
                        // Manejar el caso en que no se pueda obtener el rectángulo
                        System.Diagnostics.Debug.WriteLine($"No se pudo obtener el rectángulo del ícono. Código de error: {result}");

                        // Intentar un método de respaldo
                        iconRect = GetFallbackSystemTrayPosition();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al obtener rectángulo del ícono: {ex.Message}");

                    // Método de respaldo
                    iconRect = GetFallbackSystemTrayPosition();
                }

                return iconRect;
            }

            /// <summary>
            /// Método de respaldo para obtener la posición de la bandeja del sistema
            /// </summary>
            private static RECT GetFallbackSystemTrayPosition()
            {
                // Encuentra la ventana de la bandeja del sistema
                IntPtr systemTrayHandle = FindWindow("Shell_TrayWnd", null);
                IntPtr notificationAreaHandle = FindWindowEx(systemTrayHandle, IntPtr.Zero, "TrayNotifyWnd", null);

                RECT fallbackRect = new RECT();
                GetWindowRect(notificationAreaHandle, out fallbackRect);

                return fallbackRect;
            }

            // Métodos de respaldo adicionales
            [DllImport("user32.dll")]
            private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

            [DllImport("user32.dll")]
            private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

            [DllImport("user32.dll")]
            private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

            /// <summary>
            /// Posiciona una ventana cerca del ícono de la bandeja del sistema
            /// </summary>
            /// <param name="hWnd">Handle de la ventana a posicionar</param>
            /// <param name="width">Ancho de la ventana</param>
            /// <param name="height">Alto de la ventana</param>
            public static void PositionNearSystemTrayIcon(IntPtr hWnd, int width, int height)
            {
                try
                {
                    // Obtener el rectángulo del ícono de la bandeja
                    RECT iconRect = GetSystemTrayIconRect(hWnd);

                    // Calcular posición ajustada
                    int adjustedX = iconRect.right - width - 10;  // Ajusta este valor para mover más a la izquierda/derecha
                    int adjustedY = iconRect.top - height - 10;   // Ajusta este valor para mover más arriba/abajo

                    // Posicionar la ventana
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
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al posicionar ventana: {ex.Message}");
                }
            }
        }
}
