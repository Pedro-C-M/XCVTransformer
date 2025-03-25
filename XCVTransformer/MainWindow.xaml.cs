using System;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using WinRT.Interop;
using XCVTransformer.AuxClasses;
using XCVTransformer.Helpers;
using XCVTransformer.Pages;
using XCVTransformer.ViewModels;

namespace XCVTransformer
{
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        //Se puede configurar aqui el ancho y alto de la ventana
        const int windowWidth = 600;
        const int windowHeight = 800;

        private IntPtr hWnd;//Atr del handler de la ventana principal
        private TrayManager trayManager;

        public MainViewModel ViewModel { get; }

        //Marca la última opción del Nav seleccionada, esto para marcar el botón como selected
        private NavigationViewItem _lastSelectedItem = null;


        public MainWindow()
        {
            this.InitializeComponent();

            ViewModel = new MainViewModel();
            AsignDataContext();

            PrepareTray();
            LoadNav();
        }

        /**
         * Asigna al grid root de la ventana el DataContext del view model
         */
        private void AsignDataContext()
        {
            if(this.Content is FrameworkElement rootElement)
            {
                rootElement.DataContext = ViewModel;
            }
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
         * ---------------------------------------Handlers de eventos de la ventana-------------------------------------------------------
        */
       

        /**
         * Carga la página que quiero en inicio en el frame, en este caso TranslatorPage
         */
        private void LoadNav()
        {
            contentFrame.Navigate(typeof(TranslatorPage));
        }

        /**
         * Cada vez que un NavItem se toque, en base al sender se navegará a la página
         * correspondiente. Esto se saca del tag
         */
        private void NavItemTapped(object sender, TappedRoutedEventArgs e)
        {
            var tappedItem = sender as NavigationViewItem;

            if (tappedItem != null)
            {
                alternateLastClickNavButton(tappedItem);
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

        //---------------------------------------AUX METHODS-------------------------------------

        /**
         * Quita el anterior como selected
         * Marca el nuevo NavItem como selected 
         * Cambia el valor de lastSelectedItem que representa el último botón tocado para poder 
         * quitarle luego el selected.
         */
        private void alternateLastClickNavButton(NavigationViewItem tappedItem)
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
        }
    }
}