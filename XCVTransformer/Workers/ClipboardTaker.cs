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
            Clipboard.ContentChanged += OnClipboardContentChanged;
        }

        /**
         * Obtenemos el texto que hay en el portapapeles
         */
        private async void OnClipboardContentChanged(object sender, object e)
        {
            var package = Clipboard.GetContent();
            if (package.Contains(StandardDataFormats.Text))
            {
                string text = await package.GetTextAsync();
                ClipboardTextChanged?.Invoke(this, text);
            }
        }
    }
}
