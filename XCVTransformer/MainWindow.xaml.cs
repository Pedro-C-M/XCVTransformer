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
using XCVTransformer.AuxClasses;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace XCVTransformer
{
    public sealed partial class MainWindow : Window
    {
        private IntPtr hWnd;//Atr del handler de la ventana principal
        private TrayManager trayManager;

        public MainWindow()
        {
            this.InitializeComponent();
            hWnd = WindowNative.GetWindowHandle(this);
           
            Helpers.IconHelper.SetWindowIcon("Assets/AppLogo/App-logo.ico", hWnd);
            trayManager = new TrayManager(this, hWnd);
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