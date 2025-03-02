using System;
using System.Runtime.InteropServices;
using XCVTransformer.Helpers;

namespace XCVTransformer.WinSubclasses
{
    public class TrayWindowSubclass
    {
        // Delegado para el procedimiento de subclase
        private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        // Función para subclasificar una ventana
        [DllImport("Comctl32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowSubclass(IntPtr hWnd, WndProcSubclassProc pfnSubclass, UIntPtr uIdSubclass, UIntPtr dwRefData);

        // Función para remover la subclasificación
        [DllImport("Comctl32.dll", SetLastError = true)]
        private static extern bool RemoveWindowSubclass(IntPtr hWnd, WndProcSubclassProc pfnSubclass, UIntPtr uIdSubclass);

        // Función para enviar mensajes al procedimiento de subclase anterior
        [DllImport("Comctl32.dll", SetLastError = true)]
        private static extern IntPtr DefSubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        // Delegado para el procedimiento de subclase
        private delegate IntPtr WndProcSubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, UIntPtr uIdSubclass, UIntPtr dwRefData);

        // Campo para mantener vivo al delegado
        private static WndProcSubclassProc _wndProcDelegate;

        // El ID único para nuestra subclase
        private const uint SUBCLASS_ID = 100;

        // Evento que se dispara cuando llega un mensaje de ventana
        public static event EventHandler<WindowMessageEventArgs> WindowMessageReceived;

        public static void RegisterSubclass(IntPtr hWnd)
        {
            // Crear un delegado para el procedimiento de subclase
            _wndProcDelegate = new WndProcSubclassProc(WindowProc);

            // Subclasificar la ventana
            SetWindowSubclass(hWnd, _wndProcDelegate, (UIntPtr)SUBCLASS_ID, UIntPtr.Zero);
        }

        public static void UnregisterSubclass(IntPtr hWnd)
        {
            // Remover la subclasificación
            RemoveWindowSubclass(hWnd, _wndProcDelegate, (UIntPtr)SUBCLASS_ID);
        }

        private static IntPtr WindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, UIntPtr uIdSubclass, UIntPtr dwRefData)
        {
            // Comprobar si el mensaje es el que nos interesa (mensaje de notificación de la bandeja)
            if (uMsg == TrayHelper.WM_MOUSEMOVE)
            {
                // Disparar el evento con los detalles del mensaje
                WindowMessageReceived?.Invoke(null, new WindowMessageEventArgs
                {
                    HWnd = hWnd,
                    Msg = uMsg,
                    WParam = wParam,
                    LParam = lParam
                });
            }

            // Pasar el mensaje al procedimiento de ventana anterior
            return DefSubclassProc(hWnd, uMsg, wParam, lParam);
        }
    }

    // Clase para los argumentos del evento
    public class WindowMessageEventArgs : EventArgs
    {
        public IntPtr HWnd { get; set; }
        public uint Msg { get; set; }
        public IntPtr WParam { get; set; }
        public IntPtr LParam { get; set; }
    }
}