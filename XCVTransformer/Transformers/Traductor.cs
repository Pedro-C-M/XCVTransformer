using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace XCVTransformer.Transformers
{
    class Traductor : ITransformer
    {
        private string apiKey = Environment.GetEnvironmentVariable("MY_TRANSLATOR_API_KEY");
        private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com/";
        private static readonly string route = "/translate?api-version=3.0&to=en&from=es"; // Traducción a inglés

        public async Task<string> Transform(string toTransform)
        {      
            using (var client = new HttpClient())
            {
                // Configurar la solicitud HTTP
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", "westeurope"); // Cambia la región si es necesario

                var requestBody = new[] { new { Text = toTransform } };
                var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                // Hacer la solicitud POST a la API de Microsoft Translator
                var response = await client.PostAsync(endpoint + route, content);
                response.EnsureSuccessStatusCode();

                // Leer la respuesta JSON
                var result = await response.Content.ReadAsStringAsync();

                // Procesar el resultado de la traducción
                var translatedText = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result)[0].translations[0].text;
                return translatedText;
            }
        }
    }
}
