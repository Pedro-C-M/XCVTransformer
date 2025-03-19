using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace XCVTransformer.Workers
{
    class ClipboardTaker
    {
        public event EventHandler<string> ClipboardTextChanged;
        internal ClipboardLoader loader;

        private Boolean transformingFlag = false; //Flag para controlar bloqueos del método y no causar bucles
        public ClipboardTaker()
        {
            this.loader = new ClipboardLoader();
            Clipboard.ContentChanged += OnClipboardContentChanged;
        }

        /**
         * Obtenemos el texto que hay en el portapapeles
         * Utiliza control de bloqueo para que mientras se está transformando (cosa asíncrona)
         * no se permita volver a lanzar otra transformación
         */
        private async void OnClipboardContentChanged(object sender, object e)
        {
            if (transformingFlag) return;//Si ya estamos transformando abortamos
            transformingFlag = true;

            try
            {
                var package = Clipboard.GetContent();
                if (package.Contains(StandardDataFormats.Text))
                {
                    string text = await package.GetTextAsync();
                    loader.LoadTextToClipboard(text);
                    await ClipboardLoader.MockTransformTime(1000);
                    ClipboardTextChanged?.Invoke(this, text);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en evento de cambio de portapapeles: {ex.Message}");
            }
            finally
            {
                transformingFlag = false;
            }
        }
    }
}
