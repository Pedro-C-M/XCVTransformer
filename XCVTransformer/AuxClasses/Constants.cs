using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCVTransformer.AuxClasses
{
    public static class AppConstants
    {
        /**
         *Para la lista de idiomas de traducir disponibles 
         *
         *Son los 12 Idiomas más hablados del mundo en orden incluyendo el italiano y excluyendo árabe que no encontré su código 
         */
        public static readonly List<string> LanguageList = new List<string>
        {
            "Español",
            "Inglés",
            "Francés",
            "Alemán",
            "Japonés",
            "Italiano",
            "Portugués",
            "Chino simplificado",
            "Hindú",
            "Ruso",
            "Urdu",
            "Indonesio",
            "Suajili"
        };
        /**
         *Para la correspondencia de el código de cada idioma para la API
         */
        public static readonly Dictionary<string, string> LanguageCodes = new Dictionary<string, string>
        {
            { "Inglés", "en" },
            { "Chino simplificado", "zh-Hans" },
            { "Hindú", "hi" },
            { "Español", "es" },
            { "Francés", "fr" },
            { "Portugués", "pt-pt" },
            { "Ruso", "ru" },
            { "Urdu", "ur" },
            { "Indonesio", "id" },
            { "Suajili", "sw" },
            { "Alemán", "de" },
            { "Japonés", "ja" },
            { "Italiano", "it" }
        };
    }
}
