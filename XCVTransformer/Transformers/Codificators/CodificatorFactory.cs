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
                "Codificación Base64" => new Base64Codificator(),
                "Codificación Hexadecimal" => new HexCodificator(),
                "Codificación Morse" => new MorseCodificator(),
                "Codificacón ROT13" => new ROT13Codification(),
                "Encriptación AES" => new AESCodificator(),
                "Encriptación DES" => new DESCodificator(),
                "Encriptación Blowfish" => new BlowfishCodificator(),
                "Encriptación Enigma" => new EnigmaCodificator(),
                "Invertir orden" => new Inversor(),
                "Contar caracteres" => new CharCount(),
                _ => throw new ArgumentException($"Codificación desconocida: {codificationName}")
            };
        }

    }
}
