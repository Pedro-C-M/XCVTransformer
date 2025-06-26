using XCVTransformer.Transformers.Codificators;

namespace XCVTransformer.Tests
{

    /**
     * Clase que moquea un cliente HTTP para falsificar la llamada
     * al servicio de traducción
     * 
     * Tiene un atributo para la respuesta que vamos a esperar, un constructor con ese atributo
     * y el metodo de mock que simula una llamada y retorna ese atributo respuesta.
     * 
     */
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public MockHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_response);
        }
    }


    public class FailingHttpHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => throw new HttpRequestException("Simulated network error");
    }

    public class MockKeyStorage : IKeyStorage
    {
        private readonly Func<Task<(byte[] Key, byte[] IV)>> _behavior;

        public MockKeyStorage(Func<Task<(byte[] Key, byte[] IV)>> behavior)
        {
            _behavior = behavior;
        }

        public Task<(byte[] Key, byte[] IV)> GetOrCreateKeyAndIVAsync()
        {
            return _behavior();
        }
    }
}
