﻿using System.Threading.Tasks;
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
        public bool IsInternalUpdate { get; private set; } = false;
        private const int LockedTime = 200;


        /**
         * Este método se encarga de cargar una cadena al portapapeles
         * 
         * És síncrono SetContent()
         */
        public async Task LoadTextToClipboard(string text)
        {
            ///Para evitar bucles repitiendo lo mismo por problemas de asincronidad
            /**
            if (_transformer is not LanguageDetector && text == lastTransformedText)
            {
                return;
            }
            */
            string transformedText = await TransformText(text);
            if (isDetectorOn)
            {
                OnDetectionCompleted?.Invoke(transformedText);
            }
            else {
                DataPackage dataPackage = new DataPackage();
                dataPackage.SetText(transformedText);

                IsInternalUpdate = true; //Lock de 200ms para evitar bucles infinitos

                Clipboard.SetContent(dataPackage);
                lastTransformedText = transformedText;

                await Task.Delay(LockedTime); //Abre el lock
                IsInternalUpdate = false;
            }
        }

        internal void ReestartLastTransformedWord()
        {
            lastTransformedText = "";
        }

        public void ChangeToDetectorMode(bool detectorMode, bool isChangeBromSwitch)
        {
            this.isDetectorOn = detectorMode;
            if (detectorMode)
            {
                this.lastTransformer = _transformer;//Guardamos para reiniciar el transformer después
                _transformer = new LanguageDetector();
            }
            else
            {
                if (isChangeBromSwitch)
                {
                    this._transformer = lastTransformer;//Renovamos el transformer que había
                }
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
