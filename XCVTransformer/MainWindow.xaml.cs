using System;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using WinRT.Interop;
using XCVTransformer.AuxClasses;
using XCVTransformer.Helpers;
using XCVTransformer.Pages;
using XCVTransformer.ViewModels;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml.Media;

namespace XCVTransformer
{
    public sealed partial class MainWindow : Window
    {
        //Se puede configurar aqui el ancho y alto de la ventana
        const int windowWidth = 600;
        const int windowHeight = 800;

        private IntPtr hWnd;//Atr del handler de la ventana principal
        private TrayManager trayManager;

        public MainViewModel ViewModel { get; }
        //Marca la opción del Nav seleccionada inicialmente, esto para marcar el botón como selected y ver la page
        private string startingPage = "TranslatorPage";


        public MainWindow()
        {
            this.InitializeComponent();

            ViewModel = new MainViewModel(contentFrame);
            AsignDataContext();

            RemoveTitleBar();
            PrepareTray();
            LoadNav();
            BackdropMaterialHelper.PrepareBackdropMaterial(this);        
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

        /**
         * Quita la Title Bar para mejor diseño y evitar el movimiento de la ventana
         */
        private void RemoveTitleBar()
        {
            var hwnd = WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            AppWindow appWin = AppWindow.GetFromWindowId(windowId);

            if (appWin.Presenter is OverlappedPresenter presenter)
            {
                presenter.SetBorderAndTitleBar(true, false);
            }
        }

        /***
         * ---------------------------------------Handlers de eventos de la ventana-------------------------------------------------------
        */


        /**
         * Carga la página que quiero en inicio en el frame sincroniza con el VM, en este caso TranslatorPage
         */
        private void LoadNav()
        {
            contentFrame.Navigate(typeof(TranslatorPage));
            alternateLastClickNavButton("", startingPage);
            ViewModel.LastSelectedNavItem = startingPage;
        }

        /**
         * Cada vez que un NavItem se toque, se notifica al ViewModel para que navegue
         */
        private void NavItemTapped(object sender, TappedRoutedEventArgs e)
        {
            var tappedItem = sender as NavigationViewItem;

            if (tappedItem != null)
            {
                string lastTag = ViewModel.LastSelectedNavItem;
                string pageTag = tappedItem.Tag.ToString();
                ViewModel.Navigate(pageTag);

                alternateLastClickNavButton(lastTag, pageTag);
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
        private void alternateLastClickNavButton(string lastTag, string pageTag)
        {
            // Si hay un item previamente seleccionado, desmarcarlo
            if (lastTag != null && lastTag != "")
            {
                var lastNavItem = (NavigationViewItem)navView.FindName(lastTag);
                lastNavItem.IsSelected = false;
            }
            var newNavItem = (NavigationViewItem)navView.FindName(pageTag);
            newNavItem.IsSelected = true;
        }
    }
}