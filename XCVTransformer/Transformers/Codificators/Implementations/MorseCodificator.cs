using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XCVTransformer.Transformers.Codificators.Implementations
{
    class MorseCodificator: AbstractCodificator
    {
        // Diccionario para codificar en morse
        private static readonly Dictionary<char, string> _charToMorse = new Dictionary<char, string>()
        {
            {'A', ".-"},    {'B', "-..."},  {'C', "-.-."},  {'D', "-.."},
            {'E', "."},     {'F', "..-."},  {'G', "--."},   {'H', "...."},
            {'I', ".."},    {'J', ".---"},  {'K', "-.-"},   {'L', ".-.."},
            {'M', "--"},    {'N', "-."},    {'O', "---"},   {'P', ".--."},
            {'Q', "--.-"},  {'R', ".-."},   {'S', "..."},   {'T', "-"},
            {'U', "..-"},   {'V', "...-"},  {'W', ".--"},   {'X', "-..-"},
            {'Y', "-.--"},  {'Z', "--.."},
            {'0', "-----"}, {'1', ".----"},{'2', "..---"}, {'3', "...--"},
            {'4', "....-"}, {'5', "....."},{'6', "-...."}, {'7', "--..."},
            {'8', "---.."},{'9', "----."},
            {' ', "/"}
        };
        private static readonly Dictionary<string, char> _morseToChar = new Dictionary<string, char>();

        static MorseCodificator()
        {
            foreach (var pair in _charToMorse)
            {
                _morseToChar[pair.Value] = pair.Key;
            }
        }

        protected override Task<string> Encode(string input)
        {
            input = input.ToUpperInvariant();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (_charToMorse.TryGetValue(c, out string morse))
                {
                    sb.Append(morse);
                    if (i < input.Length - 1)
                        sb.Append(" ");
                }
                else
                {
                    // Caracter no soportado en el diccionario sale ?
                    sb.Append("? ");
                }
            }

            return Task.FromResult(sb.ToString().Trim());
        }

        protected override Task<string> Decode(string input)
        {
            StringBuilder sb = new StringBuilder();

            string[] words = input.Split(new string[] { " / " }, StringSplitOptions.None);

            for (int w = 0; w < words.Length; w++)
            {
                string[] letters = words[w].Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (string letter in letters)
                {
                    if (_morseToChar.TryGetValue(letter, out char c))
                    {
                        sb.Append(c);
                    }
                    else
                    {
                        sb.Append('?');
                    }
                }
                if (w < words.Length - 1)
                    sb.Append(' ');
            }

            return Task.FromResult(sb.ToString());
        }

        protected override string GetName()
        {
            return "Morse";
        }
    }
}
