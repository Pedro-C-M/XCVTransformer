using System;
using System.Runtime.InteropServices;

namespace XCVTransformer.Helpers
{
    /*
     * Helper que quitará las posibilidades de maximizar y minimizar ventana y redimensionar
     * la ventana.
     * 
     */
    public static class WindowLockHelper
    {
        private const uint WS_SIZEBOX = 0x00040000;
        private const uint WS_MINIMIZE = 0x20000000;
        private const uint WS_MAXIMIZE = 0x01000000;
        private const int GWL_STYLE = -16;

        [DllImport("user32.dll")]
        private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public static void LockWindowSize(IntPtr windowHandle)
        {
            //Aquí quita las caraterísticas que queremos fuera (max, min, resize) a la ventana
            uint style = GetWindowLong(windowHandle, GWL_STYLE);
            style &= ~(WS_MINIMIZE | WS_MAXIMIZE | WS_SIZEBOX); 
            SetWindowLong(windowHandle, GWL_STYLE, style);
        }
    }
}