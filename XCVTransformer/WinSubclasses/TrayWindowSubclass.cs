using System;
using System.Runtime.InteropServices;
using XCVTransformer.Helpers;

namespace XCVTransformer.WinSubclasses
{
    public class TrayWindowSubclass
    {
        /**----------------------------------------------------------------------IMPORTACIONES DE P/INVOKE--------------------------------------------------------------------------------------
         * Comctl sirve para window subclassing
         */
        [DllImport("Comctl32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowSubclass(IntPtr hWnd, WndProcSubclassProc pfnSubclass, UIntPtr uIdSubclass, UIntPtr dwRefData);

        [DllImport("Comctl32.dll", SetLastError = true)]
        private static extern bool RemoveWindowSubclass(IntPtr hWnd, WndProcSubclassProc pfnSubclass, UIntPtr uIdSubclass);

        [DllImport("Comctl32.dll", SetLastError = true)]
        private static extern IntPtr DefSubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        /**----------------------------------------------------------------------ATRIBUTOS Y CONSTANTES------------------------------------------------------------------------------------------
         * WndProcDelegate firma para el procedimiento de ventana normal, el subclass es la firma de la subclase.
         * El private static  es una referencia para evitar que el recolector de basura se lo lleve y la lie parda.
         * Es un evento al que se pueden subscribir en otras partes y recibir notificación cuando se reciban mensajes en el tray.
         * El ID identifica para cosas como borrar o registrar la windows subclass.
         */

        /**>>>>>>APUNTE SOBRE DELEGADOS Y FIRMAS DE C#<<<<<<<<
         * Los delegates o firmas son una posibilidad de C# que de alguna forma permiten establecer una clase de método,
         * de esta forma todo método que devuelva un IntrPtr y tenga esos parámetros podrá considerarse de la firma
         * WndProcDelegate o el otro.
         */
        private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        private delegate IntPtr WndProcSubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, UIntPtr uIdSubclass, UIntPtr dwRefData);
        private static WndProcSubclassProc _wndProcDelegate;
        public static event EventHandler<WindowMessageEventArgs> WindowMessageReceived;

        private const uint SUBCLASS_ID = 101;//Le pongo esto porque me acaba de pasar confundirme con el id de los NOTIFYDATA entonces los de subclases empezaran en 100

        /***----------------------------------------------------------------------MÉTODOS Y DEMÁS------------------------------------------------------------------------------------------
         * Crea el delegado para el proc de la subclase y lo subclasifica con su handler, id y delegado.
         */
        public static void RegisterSubclass(IntPtr hWnd)
        {
            _wndProcDelegate = new WndProcSubclassProc(WindowProc);
            SetWindowSubclass(hWnd, _wndProcDelegate, (UIntPtr)SUBCLASS_ID, UIntPtr.Zero);
        }

        /***
         * Inverso del registro que quita una subclase de ventana.
         */
        public static void UnregisterSubclass(IntPtr hWnd)
        {
            RemoveWindowSubclass(hWnd, _wndProcDelegate, (UIntPtr)SUBCLASS_ID);
        }

        /***
         * Este es el procedimiento de nuestra subclase que mira si es el mensaje interesado y realiza algo en base a el, 
         * por el contrario delega a la window class padre su funcionamiento normal.
         * 
         * Comprueba si es WM_MOUSEMOVE (mensaje de notificación de bandeja), si lo es activa el eventhandler, 
         * luego continua el flujo normal a DefSubclassProc (El proc del window padre).
         */
        private static IntPtr WindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, UIntPtr uIdSubclass, UIntPtr dwRefData)
        {
            if (uMsg == TrayHelper.WM_MOUSEMOVE)
            {
                WindowMessageReceived?.Invoke(null, new WindowMessageEventArgs
                {
                    HWnd = hWnd,
                    Msg = uMsg,
                    WParam = wParam,
                    LParam = lParam
                });
            }

            return DefSubclassProc(hWnd, uMsg, wParam, lParam);
        }
    }
    // Modificado para los argumentos que se pasa en el evento.
    public class WindowMessageEventArgs : EventArgs
    {
        public IntPtr HWnd { get; set; }
        public uint Msg { get; set; }
        public IntPtr WParam { get; set; }
        public IntPtr LParam { get; set; }
    }
}