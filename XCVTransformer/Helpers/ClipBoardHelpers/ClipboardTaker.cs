﻿using System;
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;
using XCVTransformer.Transformers;



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
            if (loader.IsInternalUpdate)
                return; //Descartar cambios hechos por el bucle de asincronia del loader

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

                    //Trim quita saltos de linea y caracteres de este tipo al inicio o fin del contenido, esto evita problema de transformar
                    //\n\r por ejemplo al copiar vacios y causa q codificaciones muestren algo de vacio
                    string trimmedText = text.Trim();

                    // Si tras trim el texto está vacío, no procesamos
                    if (string.IsNullOrEmpty(trimmedText))
                    {
                        transformingFlag = false;
                        return;
                    }
                    //Comprobamos longitud del texto si pasa maximo 
                    var result = this.loader._transformer.MaxCharactersAllowed(trimmedText.Length);
                    if (result.overMax)
                    {//Se pasa de limite y cortamos
                        AuxClasses.NotificationLauncher.NotifyCharactersLimit(result.accion, result.limit);
                        transformingFlag = false;
                        return;
                    }
                    await loader.LoadTextToClipboard(trimmedText);
                    //await ClipboardLoader.MockTransformTime(1000);
                    ClipboardTextChanged?.Invoke(this, trimmedText);
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
