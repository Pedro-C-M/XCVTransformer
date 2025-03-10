using System;
using System.Runtime.InteropServices;

namespace XCVTransformer.Helpers
{
    /*
     * Permite convertir la ventana de AppWindow a ToolWindow para que asi no salga en la taskbar de Windows 
     * y cumpla lo que busco con el tray. 
     * 
     * Hace uso de la API de Windows user32.dll de la cual utiliza los métodos:
     * 
     *  - Set/GetWindowLong: Cambia atributos de la ventana
     *  
     * Tomo el estilo de la ventana y le quito el flag de AppWindow para activar el de ToolWindow
     * 
     */
    static class TaskBarHelper
    {
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_APPWINDOW = 0x00040000;
        private const int WS_EX_TOOLWINDOW = 0x00000080;


        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        public static void HideFromTaskbar(IntPtr hWnd)
        {
            try
            {
                int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
                SetWindowLong(hWnd, GWL_EXSTYLE,(exStyle & ~WS_EX_APPWINDOW) | WS_EX_TOOLWINDOW);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error quitando de la barra de tareas: {ex.Message}");
            }
        }
    }
}
