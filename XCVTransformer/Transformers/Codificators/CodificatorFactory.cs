using System;

namespace XCVTransformer.Transformers.Codificators
{
    public static class CodificatorFactory
    {
        /**
         * Devuelve el codificador necesitado dado su nombre, este se ve en Constants
         * 
         */
        public static ITransformer Create(string codificationName)
        {
            return codificationName switch
            {
                "Base64" => new Base64Codificator(),
                _ => throw new ArgumentException($"Codificación desconocida: {codificationName}")
            };
        }

    }
}
