using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;

namespace XCVTransformer.Transformers
{
   public class LanguageDetector : ITransformer
    {
        private readonly HttpClient httpClient;

        private string apiKey = Environment.GetEnvironmentVariable("MY_TRANSLATOR_API_KEY");
        private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com/";
        private static readonly string apiVersion = "3.0";

        public static Dictionary<string, string> LanguageCodes = new()
        {
            { "Afrikáans", "af" },
            { "Árabe", "ar" },
            { "Bengalí", "bn" },
            { "Búlgaro", "bg" },
            { "Catalán", "ca" },
            { "Chino (simplificado)", "zh-Hans" },
            { "Chino (tradicional)", "zh-Hant" },
            { "Checo", "cs" },
            { "Coreano", "ko" },
            { "Croata", "hr" },
            { "Danés", "da" },
            { "Eslovaco", "sk" },
            { "Esloveno", "sl" },
            { "Español", "es" },
            { "Estonio", "et" },
            { "Filipino", "fil" },
            { "Finés", "fi" },
            { "Francés", "fr" },
            { "Griego", "el" },
            { "Hebreo", "he" },
            { "Hindi", "hi" },
            { "Holandés", "nl" },
            { "Húngaro", "hu" },
            { "Indonesio", "id" },
            { "Inglés", "en" },
            { "Italiano", "it" },
            { "Japonés", "ja" },
            { "Letón", "lv" },
            { "Lituano", "lt" },
            { "Malayo", "ms" },
            { "Noruego", "nb" },
            { "Persa", "fa" },
            { "Polaco", "pl" },
            { "Portugués (Brasil)", "pt" },
            { "Rumano", "ro" },
            { "Ruso", "ru" },
            { "Serbio (latino)", "sr-Latn" },
            { "Sueco", "sv" },
            { "Tamil", "ta" },
            { "Tailandés", "th" },
            { "Turco", "tr" },
            { "Ucraniano", "uk" },
            { "Urdu", "ur" },
            { "Vietnamita", "vi" },
            { "Yidis", "yi" }
        };

        public LanguageDetector()
        {
            this.httpClient = new HttpClient();
        }

        public LanguageDetector(HttpClient? httpClient = null)
        {
            this.httpClient = httpClient;
        }

        /**
        * Método que llama a la API de mi recurso traductor de Azure para detectar el idioma del texto
        * 
        * Método POST
        * En la cabecera va la API key y la región
        * En el body va solamente el texto como Text
        * Se envía y recibe la respuesta en formato JSON
        * 
        * Se devuelve el código de idioma detectado (por ejemplo, "es", "en")
        */
        public async Task<string> Transform(string toDetect)
        {
            var route = $"/detect?api-version={apiVersion}";
         
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", "westeurope");

            var requestBody = new[] { new { Text = toDetect } };
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync(endpoint + route, content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();

                var detectedLanguage = JsonConvert.DeserializeObject<dynamic>(result)[0].language;
                string detectedLanguageName = GetLanguageNameFromCode((string)detectedLanguage);

                //Debug.WriteLine($"Idioma detectado para \"{toDetect}\" -> {detectedLanguageName}");

                return detectedLanguageName;
            }
            catch (HttpRequestException)
            {
                AuxClasses.NotificationLauncher.NotifyNoInternetError();
            }
            catch (Exception ex)
            {
                AuxClasses.NotificationLauncher.NotifyDetectorError("Contactar para informar el error");
                Debug.WriteLine("Error desconocido en la traducción: " + ex.Message);
            }
            return "";
        }
        private string GetLanguageNameFromCode(string code)
        {
            var reversed = LanguageCodes.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            return reversed.TryGetValue(code, out string name) ? name : "Desconocido"; //Si no está en el diccionario muestro desconocido
        }

        public (bool, string, int) MaxCharactersAllowed(int charactersNum)
        {
            if (charactersNum < 100)
            {
                return (false, "Detectar lenguaje", 100);
            }
            return (true, "Detectar lenguaje", 100);
        }
    }
}
