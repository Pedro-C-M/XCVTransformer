using System.Net;
using System.Text;
using XCVTransformer.Transformers;


namespace XCVTransformer.Tests
{

    [TestClass]
    public class LanguageDetectorTests
    {
        [TestMethod]
        public async Task Transform_ReturnsKnownLanguageName()
        {
            var json = "[{\"language\":\"es\"}]";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var handler = new MockHttpMessageHandler(response);
            var httpClient = new HttpClient(handler);

            var detector = new LanguageDetector(httpClient);
            var result = await detector.Transform("Hola");

            Assert.AreEqual("Español", result);
        }

        [TestMethod]
        public async Task Transform_ReturnsUnknownLanguage()
        {
            var json = "[{\"language\":\"xx\"}]"; // Código no soportado
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var handler = new MockHttpMessageHandler(response);
            var httpClient = new HttpClient(handler);

            var detector = new LanguageDetector(httpClient);
            var result = await detector.Transform("Desconocido");

            Assert.AreEqual("Desconocido", result);
        }

        [TestMethod]
        public async Task Transform_OnHttpFailure()
        {
            var httpClient = new HttpClient(new FailingHttpHandler());
            var detector = new LanguageDetector(httpClient);

            var result = await detector.Transform("Fallo esperado");

            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void MaxCharactersAllowed_WhenUnderLimit()
        {
            var detector = new LanguageDetector();
            var result = detector.MaxCharactersAllowed(50);

            Assert.IsFalse(result.Item1);
            Assert.AreEqual("Detectar lenguaje", result.Item2);
            Assert.AreEqual(100, result.Item3);
        }

        [TestMethod]
        public void MaxCharactersAllowed_WhenOverLimit()
        {
            var detector = new LanguageDetector();
            var result = detector.MaxCharactersAllowed(200);

            Assert.IsTrue(result.Item1);
            Assert.AreEqual("Detectar lenguaje", result.Item2);
            Assert.AreEqual(100, result.Item3);
        }
    }
}