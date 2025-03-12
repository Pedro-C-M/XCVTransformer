using System;
using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.ApplicationModel.DataTransfer;
using WinRT.Interop;
using XCVTransformer.AuxClasses;
using XCVTransformer.Helpers;
using XCVTransformer.Pages;
using XCVTransformer.Workers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace XCVTransformer
{
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        //Se puede configurar aqui el ancho y alto de la ventana
        const int windowWidth = 600;
        const int windowHeight = 800;


        private IntPtr hWnd;//Atr del handler de la ventana principal
        private TrayManager trayManager;

        private ClipboardTaker clipboardTaker;


        public string ClipboardText { get; set; } = "Esperando texto...";

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public MainWindow()
        {
            this.InitializeComponent();
            PrepareTray();
            LoadNav();

            // Inicializar y suscribirse al ClipboardListener
            clipboardTaker = new ClipboardTaker();
            clipboardTaker.ClipboardTextChanged += OnClipboardTextChanged;
        }

        /**
         * Prepara todo lo relativo al tray:
         * - Pone icono
         * - Quita icono de la barra de tareas
         * - Prepara el manejo de eventos del icono del tray
         * - Hace que la ventana se abierta cerca del tray
         * - Quita la posibilidad de maximizar minimizar y redimensionar la ventana
         */
        private void PrepareTray()
        {
            hWnd = WindowNative.GetWindowHandle(this);

            IconHelper.SetWindowIcon("Assets/AppLogo/App-logo.ico", hWnd);
            TaskBarHelper.HideFromTaskbar(hWnd);
            trayManager = new TrayManager(this, hWnd);

            WindowPositionHelper.PositionNearSystemTrayIcon(hWnd, windowWidth, windowHeight);
            WindowLockHelper.LockWindowSize(hWnd);
        }

        /***
         * Handlers de eventos de la ventana
        */

        // Método que recibe el evento con el texto copiado
        private void OnClipboardTextChanged(object sender, string text)
        {
            // Asegura que la actualización se realice en el hilo principal de la UI
            DispatcherQueue.TryEnqueue(() =>
            {
                ClipboardText = text;
                OnPropertyChanged(nameof(ClipboardText)); // Notificar que la propiedad cambió
            });
        }

        private void LoadNav()
        {
            contentFrame.Navigate(typeof(TranslatorPage));
        }

        private NavigationViewItem _lastSelectedItem = null; // Variable para almacenar el último item seleccionado
        private void NavItemTapped(object sender, TappedRoutedEventArgs e)
        {
            var tappedItem = sender as NavigationViewItem;

            if (tappedItem != null)
            {
                // Si hay un item previamente seleccionado, desmarcarlo
                if (_lastSelectedItem != null)
                {
                    _lastSelectedItem.IsSelected = false;
                }

                // Marcar el nuevo item como seleccionado
                tappedItem.IsSelected = true;

                // Actualizar el último item seleccionado
                _lastSelectedItem = tappedItem;

                string tag = tappedItem.Tag.ToString();

                if (tag == "TranslatorPage")
                {
                    contentFrame.Navigate(typeof(TranslatorPage));
                }
                else if (tag == "OptionsPage")
                {
                    contentFrame.Navigate(typeof(OptionsPage));
                }
            }
        }
        /**
        private void RevealModeCheckbox_Changed(object sender, RoutedEventArgs e)
        {
            if (revealModeCheckBox.IsChecked == true)
            {
                passworBoxWithRevealmode.PasswordRevealMode = PasswordRevealMode.Hidden;
            }
            else
            {
                passworBoxWithRevealmode.PasswordRevealMode = PasswordRevealMode.Visible;
            }
        }
        */
    }
}