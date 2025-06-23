using System;
using System.Linq;
using System.Threading.Tasks;

namespace XCVTransformer.Transformers.Codificators.Implementations
{
    /**
     * Encriptación Enigma basada en la de la WW2 simplificada. Es interesante pero no muy seguro
     * 
     */
    class EnigmaCodificator : AbstractCodificator
    {
        private readonly string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private readonly int[] rotor = { 4, 10, 12, 5, 11, 6, 3, 16, 21, 25, 13, 19, 14, 22, 24, 7, 23, 20, 18, 15, 0, 8, 1, 17, 2, 9 };

        private char EncodeChar(char input)
        {
            int index = alphabet.IndexOf(char.ToUpper(input));
            if (index == -1) return input; // ignorar caracteres no alfabéticos

            int mappedIndex = rotor[index];
            return alphabet[mappedIndex];
        }

        private char DecodeChar(char input)
        {
            int index = alphabet.IndexOf(char.ToUpper(input));
            if (index == -1) return input;

            int mappedIndex = Array.IndexOf(rotor, index);
            return alphabet[mappedIndex];
        }

        protected override Task<string> Encode(string input)
        {
            var encoded = new string(input.Select(EncodeChar).ToArray());
            return Task.FromResult(encoded);
        }

        protected override Task<string> Decode(string input)
        {
            var decoded = new string(input.Select(DecodeChar).ToArray());
            return Task.FromResult(decoded);
        }

        protected override string GetName() => "Encriptación Enigma";


    }
}
