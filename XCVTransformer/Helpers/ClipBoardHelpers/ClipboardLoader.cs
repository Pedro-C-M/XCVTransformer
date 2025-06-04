using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using System.Diagnostics;
using XCVTransformer.Transformers;
using System;

namespace XCVTransformer.Helpers
{
    class ClipboardLoader
    {
        internal ITransformer _transformer = new Traductor();

        //Para el detector
        private ITransformer lastTransformer;
        private bool isDetectorOn;
        public event Action<string> OnDetectionCompleted;


        private string lastTransformedText = "";
        /**
         * Este método se encarga de cargar una cadena al portapapeles
         * 
         * És síncrono SetContent()
         */
        public async Task LoadTextToClipboard(string text)
        {
            ///Para evitar bucles repitiendo lo mismo por problemas de asincronidad
            if (!isDetectorOn && text == lastTransformedText)
            {
                return;
            }
            string transformedText = await TransformText(text);
            if (isDetectorOn)
            {
                OnDetectionCompleted?.Invoke(transformedText);
            }
            else {
                DataPackage dataPackage = new DataPackage();
                dataPackage.SetText(transformedText); 

                Clipboard.SetContent(dataPackage);
                lastTransformedText = transformedText;
            }
        }

        internal void ReestartLastTransformedWord()
        {
            lastTransformedText = "";
        }

        public void ChangeToDetectorMode(bool detectorMode)
        {
            this.isDetectorOn = detectorMode;
            if (detectorMode)
            {
                this.lastTransformer = _transformer;//Guardamos para reiniciar el transformer después
                _transformer = new LanguageDetector();
            }
            else
            {
                this._transformer = lastTransformer;//Renovamos el transformer que había
            }
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
