using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;

using XCVTransformer.Helpers;
using XCVTransformer.WinSubclasses;

using System.Runtime.InteropServices;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace XCVTransformer
{
    public sealed partial class MainWindow : Window
    {
        private IntPtr hWnd;//Atr del handler de la ventana principal

        public MainWindow()
        {
            this.InitializeComponent();
            hWnd = WindowNative.GetWindowHandle(this);
           
            Helpers.IconHelper.SetWindowIcon("Assets/AppLogo/App-logo.ico", hWnd);
            
            Helpers.TrayHelper.ShowTrayIcon(hWnd, "Estoy vivo", "Assets/AppLogo/App-logo.ico");

            WinSubclasses.TrayWindowSubclass.RegisterSubclass(hWnd);
            WinSubclasses.TrayWindowSubclass.WindowMessageReceived += OnWindowMessageReceived;

            this.Closed += MainWindow_Closed;
        }
        /***
         * Si el mensaje recibido es del tray icon ir a su handler, aqui se llega desde la subclass cuando le llega WindowMessageReceived manda a aqui
         */
        private void OnWindowMessageReceived(object sender, WindowMessageEventArgs e)
        {
            if (e.WParam.ToInt32() == 1) // 1 es el uID del NOTIFYICONDATA del tray
            {
                TrayHelper.HandleTrayMessage(hWnd, (uint)e.LParam);
            }
        }
        /***
         * Importante limpiar recursos de tray para que no se quede el icono en el area de tray después de cerrado el programa
         */
        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {         
            WinSubclasses.TrayWindowSubclass.WindowMessageReceived -= OnWindowMessageReceived;
            WinSubclasses.TrayWindowSubclass.UnregisterSubclass(hWnd);
            TrayHelper.RemoveTrayIcon(this);
        }

        /***
         * Click sin mas jej
         */
        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content += "Clicked";
        }
    }
}