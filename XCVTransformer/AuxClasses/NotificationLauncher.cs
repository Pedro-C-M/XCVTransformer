using Microsoft.Toolkit.Uwp.Notifications;

namespace XCVTransformer.AuxClasses
{
    static class NotificationLauncher
    {
        internal static void NotifyNoInternetError()
        {
            new ToastContentBuilder()
                .AddText("Error de conexión al traducir")
                .AddText("No hay conexión a internet por lo que no se pueden traducir los textos")
                .Show();
        }

        /**
         * Se lanza cuando se intenta traducir teniendo mismo idioma origen y destino
         */
        internal static void NotifySameOriginAndEndLanguage()
        {
            new ToastContentBuilder()
                .AddText("No se ha traducido")
                .AddText("Ha seleccionado el mismo lenguaje origen que destino por lo que no se está transformando nada")
                .Show();
        }

        internal static void NotifyTransformerIsOff()
        {
            new ToastContentBuilder()
                .AddText("XCVTransformer está desactivado")
                .AddText("Tiene XCVTransformer desactivado por lo que no se transformará nada")
                .Show();
        }

    }
}
