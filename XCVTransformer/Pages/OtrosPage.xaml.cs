using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace XCVTransformer.Pages
{ 
    public sealed partial class OtrosPage : Page
    {
        public OtrosPage()
        {
            this.InitializeComponent();
        }

        private void CerrarApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}
