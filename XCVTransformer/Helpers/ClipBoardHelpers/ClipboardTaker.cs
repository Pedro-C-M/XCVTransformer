using System;
using Windows.ApplicationModel.DataTransfer;

namespace XCVTransformer.Helpers
{
    public class ClipboardTaker
    {
        private bool transformerOn;//Flag para saber si está encendido o no el transformador

        public event EventHandler<string> ClipboardTextChanged;
        internal ClipboardLoader loader;

        private bool transformingFlag = false; //Flag para controlar bloqueos del método y no causar bucles
        public ClipboardTaker()
        {
            loader = new ClipboardLoader();
            Clipboard.ContentChanged += OnClipboardContentChanged;
        }

        internal void ChangeTransformerState(bool newState)
        {
            this.transformerOn = newState;
        }

        /**
         * Obtenemos el texto que hay en el portapapeles
         * Utiliza control de bloqueo para que mientras se está transformando (cosa asíncrona)
         * no se permita volver a lanzar otra transformación
         */
        private async void OnClipboardContentChanged(object sender, object e)
        {
            if (!this.transformerOn) return;//Si está apagado el trasnformador fuera
            if (transformingFlag) return;//Si ya estamos transformando abortamos
            transformingFlag = true;

            try
            {
                var package = Clipboard.GetContent();
                if (package.Contains(StandardDataFormats.Text))
                {
                    string text = await package.GetTextAsync();
                    await loader.LoadTextToClipboard(text);
                    //await ClipboardLoader.MockTransformTime(1000);
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
