using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace XCVTransformer.Transformers
{
    class Traductor : ITransformer
    {
        private string fromLanguage = "es";
        private string toLanguage = "en";

        private string apiKey = Environment.GetEnvironmentVariable("MY_TRANSLATOR_API_KEY");
        private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com/";

        /**
         * Método que llama a la API de mi recurso traductor de Azure, depende de los atributos to y from los idiomas a destino y origen.
         * 
         * Método POST
         * En la cabezera va la API key y la región
         * En el body va solamente el texto a transformar como Text
         * Se envía y recibe la respuesta en formato JSON
         * 
         * Se devuelve el texto traducido
         */
        public async Task<string> Transform(string toTransform)
        {
            var route = $"/translate?api-version=3.0&from={fromLanguage}&to={toLanguage}";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", "westeurope");

                var requestBody = new[] { new { Text = toTransform } };
                var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(endpoint + route, content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();

                var translatedText = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result)[0].translations[0].text;

                Debug.WriteLine("Nueva traducción de "+ toTransform);

                return translatedText;
            }
        }
    }
}
