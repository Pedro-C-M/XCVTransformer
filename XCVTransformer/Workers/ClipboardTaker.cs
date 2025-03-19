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


        private Boolean transformingFlag = false; //Flag para controlar bloqueos del método y no causar bucles
        public ClipboardTaker()
        {
            Clipboard.ContentChanged += OnClipboardContentChanged;
        }

        /**
         * Obtenemos el texto que hay en el portapapeles
         */
        private async void OnClipboardContentChanged(object sender, object e)
        {
            if (transformingFlag)
            {
                return;
            }
            transformingFlag = true;

            try
            {
                var package = Clipboard.GetContent();
                if (package.Contains(StandardDataFormats.Text))
                {
                    string text = await package.GetTextAsync();
                    //ClipboardLoader.LoadTextToClipboard(text);
                    await ClipboardLoader.MockTransformTime(1000);
                    ClipboardTextChanged?.Invoke(this, text);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al acceder al portapapeles: {ex.Message}");
            }
            finally
            {
                transformingFlag = false;
            }
        }
    }
}
