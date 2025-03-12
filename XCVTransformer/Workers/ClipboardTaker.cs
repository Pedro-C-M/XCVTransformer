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

        public ClipboardTaker()
        {
            // Suscribirse al evento del portapapeles
            Clipboard.ContentChanged += OnClipboardContentChanged;
        }

        private async void OnClipboardContentChanged(object sender, object e)
        {
            var package = Clipboard.GetContent();
            if (package.Contains(StandardDataFormats.Text))
            {
                string text = await package.GetTextAsync();
                ClipboardTextChanged?.Invoke(this, text); // Disparar el evento con el texto copiado
            }
        }
    }
}
