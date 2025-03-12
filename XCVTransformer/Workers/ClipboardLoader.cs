using Windows.ApplicationModel.DataTransfer;

namespace XCVTransformer.Workers
{
    class ClipboardLoader
    {
        /**
         * Este método se encarga de cargar una cadena al portapapeles
         */
        public static void LoadTextToClipboard(string text)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(text); 

            Clipboard.SetContent(dataPackage);
        }
    }
}
