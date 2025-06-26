using System.Text;
using XCVTransformer.Transformers;

namespace XCVTransformer.Tests
{
    public interface ITranslatorService
    {
        Task<string> TranslateAsync(string text,string orignLanguage, string targetLanguage);
    }

    [TestClass]
    public class TranslatorTests
    {
        [TestMethod]
        public async Task Transform_ReturnsExpectedTranslation()
        {
            var mockJson = "[{\"translations\":[{\"text\":\"Hello\"}]}]";
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(mockJson, Encoding.UTF8, "application/json")
            };

            var handler = new MockHttpMessageHandler(response);
            var httpClient = new HttpClient(handler);
            var traductor = new Traductor("fake-key", httpClient);

            var result = await traductor.Transform("Hola");

            Assert.AreEqual("Hello", result);
        }


        [TestMethod]
        public async Task Transform_WhenHttpFails()
        {
            var httpClient = new HttpClient(new FailingHttpHandler());
            var traductor = new Traductor("fake-key", httpClient);

            var result = await traductor.Transform("Hola");

            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void SameFromTo_WhenLanguagesAreEqual()
        {
            ITraductor traductor = new Traductor();
            traductor.ChangeOriginCode("es");
            traductor.ChangeEndCode("es");

            Assert.IsTrue(traductor.SameFromTo());
        }

        [TestMethod]
        public void SameFromTo_WhenLanguagesAreDifferent()
        {
            ITraductor traductor = new Traductor();
            traductor.ChangeOriginCode("es");
            traductor.ChangeEndCode("en");

            Assert.IsFalse(traductor.SameFromTo());
        }

        [TestMethod]
        public void MaxCharactersAllowed_WhenUnderLimit()
        {
            var traductor = new Traductor();
            var result = traductor.MaxCharactersAllowed(1500);

            Assert.IsFalse(result.Item1); // No supera límite
            Assert.AreEqual("Traducir", result.Item2);
            Assert.AreEqual(2000, result.Item3);
        }

        [TestMethod]
        public void MaxCharactersAllowed_WhenOverLimit()
        {
            var traductor = new Traductor();
            var result = traductor.MaxCharactersAllowed(2500);

            Assert.IsTrue(result.Item1); // Supera límite
        }
    }
}