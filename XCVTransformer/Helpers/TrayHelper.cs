﻿using System;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using WinRT.Interop;

namespace XCVTransformer.Helpers
{
    public class TrayHelper
    {
        // Estructura de la notificación para el ícono de la bandeja
        [StructLayout(LayoutKind.Sequential)]
        public struct NOTIFYICONDATA
        {
            public uint cbSize;
            public IntPtr hwnd;
            public uint uID;
            public uint uFlags;
            public uint uCallbackMessage;
            public IntPtr hIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szTip;
        }

        // Constantes para los flags
        private const uint NIF_MESSAGE = 0x00000001;
        private const uint NIF_ICON = 0x00000002;
        private const uint NIF_TIP = 0x00000004;
        private const uint NIF_SHOWTIP = 0x00000080;

        public const uint WM_MOUSEMOVE = 0x2000; // Mover
        public const uint WM_LBUTTONUP = 0x0202; // Clic izd
        public const uint WM_RBUTTONUP = 0x0205; // Clic derecho


        private const uint IMAGE_ICON = 1;
        private const uint LR_LOADFROMFILE = 0x0010;
        private const uint TPM_LEFTALIGN = 0x0000;
        private const uint TPM_LEFTBUTTON = 0x0000;
        private const uint TPM_RETURNCMD = 0x0100;



        // Importar las funciones necesarias desde las bibliotecas de Windows
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr LoadIcon(IntPtr hInstance, string lpIconName);

        [DllImport("shell32.dll", SetLastError = true)]
        public static extern IntPtr Shell_NotifyIcon(uint dwMessage, ref NOTIFYICONDATA lpData);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr LoadImage(IntPtr hinst, string lpszName, uint uType, int cx, int cy, uint fuLoad);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);

        [DllImport("user32.dll")]
        private static extern IntPtr CreatePopupMenu();

        [DllImport("user32.dll")]
        private static extern bool AppendMenu(IntPtr hMenu, uint uFlags, uint uIDNewItem, string lpNewItem);

        [DllImport("user32.dll")]
        private static extern bool DestroyMenu(IntPtr hMenu);

        // Estructura para obtener la posición del cursor
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        public static void ShowTrayIcon(IntPtr hwnd, string tooltip, string iconPath)
        {
            // Crear una estructura para la notificación
            NOTIFYICONDATA nid = new NOTIFYICONDATA();
            nid.cbSize = (uint)Marshal.SizeOf(typeof(NOTIFYICONDATA));
            nid.hwnd = hwnd; // Usamos GetWindowHandle para obtener el handle
            nid.uID = 1;
            nid.uFlags = NIF_ICON | NIF_TIP | NIF_MESSAGE;
            nid.uCallbackMessage = WM_MOUSEMOVE;
            nid.hIcon = LoadImage(IntPtr.Zero, iconPath, IMAGE_ICON, 32, 32, LR_LOADFROMFILE);
            nid.szTip = tooltip;

            // Agregar el ícono a la bandeja del sistema
            Shell_NotifyIcon(0x00000000, ref nid); // Añadir el ícono a la bandeja
        }

        public static void HandleTrayMessage(IntPtr hwnd, uint msg, MainWindow mainWin)
        {
            if (msg == WM_RBUTTONUP) // Clic derecho menu contextual
            {
                ShowContextMenu(hwnd, mainWin);
            }
            if (msg == WM_LBUTTONUP) // Clic izdo minimizo/abro ventana
            {
                OpenOrCloseMainWindow(mainWin);
            }
        }

        private static void ShowContextMenu(IntPtr hwnd, MainWindow mainWin)
        {
            // Obtener la posición actual del cursor
            if (!GetCursorPos(out POINT cursorPos))
                return;

            IntPtr hMenu = CreatePopupMenu();

            // Agregar opciones al menú
            AppendMenu(hMenu, 0, 1, "Abrir XCVTransformer");
            AppendMenu(hMenu, 0, 2, "Salir");

            // Hacer que el menú aparezca en primer plano
            SetForegroundWindow(hwnd);

            // Mostrar el menú en la posición del cursor
            uint clicked = (uint)TrackPopupMenu(hMenu, TPM_LEFTALIGN | TPM_LEFTBUTTON | TPM_RETURNCMD, cursorPos.X, cursorPos.Y, 0, hwnd, IntPtr.Zero);

            //Debug.WriteLine($"Opción seleccionada: {clicked}");

            // Procesar selección del menú
            switch (clicked)
            {
                case 1:
                    if(mainWin == null)//Por seguridad
                    {
                        //Debug.WriteLine("Abrir la aplicacion del menu da error de mainWin nula");
                        break;
                    }
                    //Debug.WriteLine("Abrir aplicación seleccionado");
                    OpenOrCloseMainWindow(mainWin);
                    break;
                case 2:
                    //Debug.WriteLine("Salir seleccionado en el menu contextual");
                    var app = (App)Application.Current;
                    app.ExitApplication();
                    break;
            }
            DestroyMenu(hMenu);
        }

        private static void OpenOrCloseMainWindow(MainWindow mainWin)
        {
            if(mainWin.Visible)
            {
                mainWin.AppWindow.Hide();
            }
            else
            {
                mainWin.AppWindow.Show();
                // Quiero la ventana siempre al frente al abrirla
                mainWin.Activate(); 
                SetForegroundWindow(WindowNative.GetWindowHandle(mainWin)); 
            }
        }

        public static void RemoveTrayIcon(Window window)
        {
            NOTIFYICONDATA nid = new NOTIFYICONDATA();
            nid.cbSize = (uint)Marshal.SizeOf(typeof(NOTIFYICONDATA));
            nid.hwnd = WindowNative.GetWindowHandle(window); // Usamos GetWindowHandle para obtener el handle
            nid.uID = 1;

            // Eliminar el ícono de la bandeja
            Shell_NotifyIcon(0x00000002, ref nid); // Eliminar el ícono de la bandeja
        }
    }
}