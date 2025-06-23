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
        /**
         * Errores en deteccion de idioma
         */
        internal static void NotifyDetectorError(string errorText)
        {
            new ToastContentBuilder()
                .AddText("Error detectando idioma")
                .AddText("Texto del error: \n"+ errorText)
                .Show();
        }

        /**
         * Errores de formato en codificación
         */
        internal static void NotifyBadFormatForCodification(string codName)
        {
            new ToastContentBuilder()
                .AddText("Texto entrante incorrecto")
                .AddText("No tiene formato correcto para poder transformar "+ codName)
                .Show();
        }

        internal static void NotifyCryptoError(string codName)
        {
            new ToastContentBuilder()
                .AddText("Error criptográfico")
                .AddText("Error criptográfico para poder transformar " + codName)
                .Show();
        }

        /**
         * Límite de caracteres por llamada
         */
        internal static void NotifyCharactersLimit(string accion, int nCharacteres)
        {
            new ToastContentBuilder()
               .AddText("Límite de caracteres de transformación")
               .AddText("No se puede " + accion + " con más de " + nCharacteres +" caracteres")
               .Show();
        }


    }
}
