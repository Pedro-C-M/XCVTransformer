using System.Threading.Tasks;
using System;
using Windows.ApplicationModel.DataTransfer;
using System.Diagnostics;
using XCVTransformer.Transformers;

namespace XCVTransformer.Workers
{
    class ClipboardLoader
    {
        private ITransformer _transformer = new Traductor();
        /**
         * Este método se encarga de cargar una cadena al portapapeles
         * 
         * És síncrono SetContent()
         */
        public async Task LoadTextToClipboard(string text)
        {
            string transformedText = await TransformText(text);

            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(transformedText); 

            Clipboard.SetContent(dataPackage);
        }


        public static async Task MockTransformTime(int ms)
        {
            await Task.Delay(ms);
            Debug.WriteLine($"Esperado durante {ms} milisegundos");
        }

        private async Task<string> TransformText(string toTransform)
        {
            string transformedText = await _transformer.Transform(toTransform);

            // Retornar el texto transformado
            return transformedText;
        }
    }
}
