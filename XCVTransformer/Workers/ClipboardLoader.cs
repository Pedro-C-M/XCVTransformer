using System.Threading.Tasks;
using System;
using Windows.ApplicationModel.DataTransfer;
using System.Diagnostics;

namespace XCVTransformer.Workers
{
    class ClipboardLoader
    {
        /**
         * Este método se encarga de cargar una cadena al portapapeles
         * 
         * És síncrono SetContent()
         */
        public static void LoadTextToClipboard(string text)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(text); 

            Clipboard.SetContent(dataPackage);
        }


        public static async Task MockTransformTime(int ms)
        {
            await Task.Delay(ms);
            Debug.WriteLine($"Esperado durante {ms} milisegundos");
        }
    }
}
