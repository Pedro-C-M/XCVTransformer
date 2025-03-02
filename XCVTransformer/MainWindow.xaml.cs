using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;

using XCVTransformer.Helpers;
using System.Runtime.InteropServices;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace XCVTransformer
{
    public sealed partial class MainWindow : Window
    {
        private IntPtr hWnd;//Atr del handler de la ventana principal
        private WindowSubclass windowSubclass;

        public MainWindow()
        {
            this.InitializeComponent();
            hWnd = WindowNative.GetWindowHandle(this);
            //Carga la imagen de icono de la ventana
            Helpers.IconHelper.SetWindowIcon("Assets/AppLogo/App-logo.ico", hWnd);
            //Pone el logo en el tray area 
            Helpers.TrayHelper.ShowTrayIcon(hWnd, "Estoy vivo", "Assets/AppLogo/App-logo.ico");
            // Aplicar la subclase para capturar eventos de la bandeja
            windowSubclass = new WindowSubclass(hWnd, (hwnd, msg, wParam, lParam, handledPtr) =>
            {
                bool handled = false;
                IntPtr result = WndProc(hwnd, msg, wParam, lParam, ref handled);
                return result;
            });

        }

        // Procedimiento de ventana para capturar eventos del tray
        private IntPtr WndProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x2000) // Mensaje de la bandeja del sistema
            {
                Helpers.TrayHelper.HandleTrayMessage(hwnd, (uint)lParam);
                handled = true;
            }

            return IntPtr.Zero;
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content += "Clicked";
        }
    }

    // Clase para aplicar una subclase a la ventana (capturar eventos)
    public class WindowSubclass
    {
        private delegate IntPtr WndProcDelegate(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam, ref bool handled);
        private IntPtr hWnd;
        private WndProcDelegate newWndProc;
        private IntPtr oldWndProc;

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        private const int GWL_WNDPROC = -4;

        // ?? Nueva firma del constructor
        public WindowSubclass(IntPtr hwnd, WndProcDelegate customWndProc)
        {
            hWnd = hwnd;
            newWndProc = (hwnd, msg, wParam, lParam) =>
            {
                bool handled = false;
                return customWndProc(hwnd, msg, wParam, lParam, ref handled);
            };

            oldWndProc = SetWindowLongPtr(hWnd, GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(newWndProc));
        }
    }
}