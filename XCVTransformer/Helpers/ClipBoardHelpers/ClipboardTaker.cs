using System;
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;
using XCVTransformer.Transformers;

using Microsoft.Toolkit.Uwp.Notifications; // Necesario
using Windows.UI.Notifications;
using Microsoft.Windows.AppLifecycle;



namespace XCVTransformer.Helpers
{
    public class ClipboardTaker
    {
        private bool transformerOn = true;//Flag para saber si está encendido o no el transformador

        public event EventHandler<string> ClipboardTextChanged;
        internal ClipboardLoader loader;

        private bool transformingFlag = false; //Flag para controlar bloqueos del método y no causar bucles
        private bool firstTryWhileOffFlag = true; //Flag para indicar si es la primera vez que copiamos despues de detectar apagado el transformer, para lanzar un aviso
        public ClipboardTaker()
        {
            loader = new ClipboardLoader();
            Clipboard.ContentChanged += OnClipboardContentChanged;
        }

        internal ITransformer GetTransformer()
        {
            return loader._transformer;
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
            if (!this.transformerOn)//Si está apagado el trasnformador fuera, si es la primera vez que se intenta lanzar un aviso quizas.
            {
                if (firstTryWhileOffFlag)
                {
                    this.firstTryWhileOffFlag = false;
                    AuxClasses.NotificationLauncher.NotifyTransformerIsOff();
                    return;
                }
                return;
            }
            this.firstTryWhileOffFlag = true; //Reiniciamos el flag para la proxima vez que se apague el transformador
            if (transformingFlag) return;//Si ya estamos transformando abortamos

            //Si estamos intentando traducir al mismo idioma abortamos con una notificación al usuario
            if (this.loader._transformer is ITraductor traductor)
            {
                if (traductor.SameFromTo())
                {
                    AuxClasses.NotificationLauncher.NotifySameOriginAndEndLanguage();
                    return;
                }
            }

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
                Debug.WriteLine($"Error en evento de cambio de portapapeles: {ex.Message}");
            }
            finally
            {
                transformingFlag = false;
            }
        }
    }
}
