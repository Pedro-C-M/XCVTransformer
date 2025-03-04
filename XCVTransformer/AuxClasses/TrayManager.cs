using System;
using Microsoft.UI.Xaml;

using XCVTransformer.Helpers;
using XCVTransformer.WinSubclasses;

namespace XCVTransformer.AuxClasses
{
    /**
     * Esta clase ayuda a modularizar y separar la lógica del tray de MainWindow, hace uso de TrayHelper para ello y maneja
     * todo lo relativo a crear el icono de tray y manejar su menu contextual y eventos
     */
    class TrayManager
    {
        private IntPtr hWnd;
        private MainWindow window;

        public TrayManager(MainWindow window, IntPtr hWnd)
        {
            this.window = window;
            this.hWnd = hWnd;

            SetUpTray("XCVTransformer", "Assets/AppLogo/App-logo.ico");
        }

        private void SetUpTray(string trayTooltip, string iconPath)
        {
            TrayHelper.ShowTrayIcon(hWnd, trayTooltip, iconPath);
            TrayWindowSubclass.RegisterSubclass(hWnd);

            //Subscripcion a eventos
            TrayWindowSubclass.WindowMessageReceived += OnWindowMessageReceived;
            window.Closed += MainWindow_Closed;

        }
       /***
        * Si el mensaje recibido es del tray icon ir a su handler, aqui se llega desde la subclass cuando le llega WindowMessageReceived manda a aqui
        */
        private void OnWindowMessageReceived(object sender, WindowMessageEventArgs e)
        {
            if (e.WParam.ToInt32() == 1) // 1 es el uID del NOTIFYICONDATA del tray
            {
                TrayHelper.HandleTrayMessage(hWnd, (uint)e.LParam, window);
            }
        }
       /***
        * Importante limpiar recursos de tray para que no se quede el icono en el area de tray después de cerrado el programa
        */
        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            WinSubclasses.TrayWindowSubclass.WindowMessageReceived -= OnWindowMessageReceived;
            WinSubclasses.TrayWindowSubclass.UnregisterSubclass(hWnd);
            TrayHelper.RemoveTrayIcon(window);
        }
    }

}
