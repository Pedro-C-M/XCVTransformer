using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using System.Diagnostics;
using XCVTransformer.Transformers;

namespace XCVTransformer.Helpers
{
    class ClipboardLoader
    {
        private ITransformer _transformer = new Traductor();

        private string lastTransformedText = "";
        /**
         * Este método se encarga de cargar una cadena al portapapeles
         * 
         * És síncrono SetContent()
         */
        public async Task LoadTextToClipboard(string text)
        {   ///Para evitar bucles repitiendo lo mismo por problemas de asincronidad
            if (text == lastTransformedText){
                return;
            }
            string transformedText = await TransformText(text);

            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(transformedText); 

            Clipboard.SetContent(dataPackage);
            lastTransformedText = transformedText;
            
        }

        /**
         * Usado para probar y mockear la espera de una API, testeo de bucles de clipboardChange
         */
        public static async Task MockTransformTime(int ms)
        {
            await Task.Delay(ms);
            Debug.WriteLine($"Esperado durante {ms} milisegundos");
        }

        /**
         * Llamada al método transformador
         */
        private async Task<string> TransformText(string toTransform)
        {
            return await _transformer.Transform(toTransform);
        }
    }
}
