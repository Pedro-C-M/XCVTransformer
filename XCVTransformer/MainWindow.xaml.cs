using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;


using System.Runtime.InteropServices;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace XCVTransformer
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            SetWindowIcon("Assets/AppLogo/Developing-logo.ico");
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content += "Clicked";
        }


        private void SetWindowIcon(string iconPath)
        {
            // Obtener el handle de la ventana (HWND)
            IntPtr hwnd = WindowNative.GetWindowHandle(this);

            // Cargar el icono desde el archivo
            IntPtr hIcon = LoadImage(IntPtr.Zero, iconPath, IMAGE_ICON, 32, 32, LR_LOADFROMFILE);

            // Enviar el mensaje para cambiar el icono de la ventana
            SendMessage(hwnd, WM_SETICON, (IntPtr)ICON_BIG, hIcon);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr LoadImage(IntPtr hinst, string lpszName, uint uType, int cx, int cy, uint fuLoad);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const uint WM_SETICON = 0x0080;
        private const uint ICON_BIG = 1;
        private const uint IMAGE_ICON = 1;
        private const uint LR_LOADFROMFILE = 0x0010;
    }
}