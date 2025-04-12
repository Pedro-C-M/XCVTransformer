using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinRT;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;

namespace XCVTransformer.Helpers
{
    static class BackdropMaterialHelper
    {
        static internal void PrepareBackdropMaterial(MainWindow mainWindow)
        {
            if (IsMicaSupported())
            {
                TrySetMicaBackdrop(mainWindow);
            }
            else
            {
                TrySetAcrylicBackdrop(mainWindow); // alternativa para Windows 10
            }
        }
        static private bool IsMicaSupported()
        {
            return OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000); // Windows 11+
        }

        static private void TrySetMicaBackdrop(MainWindow mainWindow)
        {
            MicaController micaController;
            SystemBackdropConfiguration config;

            micaController = new MicaController();
            config = new SystemBackdropConfiguration()
            {
                IsInputActive = true,
                Theme = SystemBackdropTheme.Default
            };

            micaController.AddSystemBackdropTarget(mainWindow.As<ICompositionSupportsSystemBackdrop>());
            micaController.SetSystemBackdropConfiguration(config);
        }

        static private void TrySetAcrylicBackdrop(MainWindow mainWindow)
        {
            var acrylicBrush = new AcrylicBrush()
            {             
                TintColor = Colors.LightBlue,
                TintOpacity = 0.6,
                FallbackColor = Colors.Gray
            };

            // Aplicamos el AcrylicBrush al fondo de la ventana
            mainWindow = acrylicBrush; 
            positionSupportsSystemBackdrop >());
            acrylicController.SetSystemBackdropConfiguration(config);
        }
    }
}
