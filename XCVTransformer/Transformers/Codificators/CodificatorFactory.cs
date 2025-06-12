using System;
using XCVTransformer.Transformers.Codificators.Implementations;

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
                "Encriptación AES" => new AESCodificator(),
                "Encriptación DES" => new DESCodificator(),
                "Encriptación Blowfish" => new BlowfishCodificator(),
                "Encriptación Enigma" => new EnigmaCodificator(),
                _ => throw new ArgumentException($"Codificación desconocida: {codificationName}")
            };
        }

    }
}
