using System;
using System.Text;
using System.Threading.Tasks;

namespace XCVTransformer.Transformers.Codificators.Implementations
{
    class ROT13Codification : AbstractCodificator
    {
        internal override Task<string> Encode(string input)
        {
            return Task.FromResult(Rot13Transform(input));
        }
        // ROT13 es simétrico: encode y decode son iguales
        internal override Task<string> Decode(string input)
        {
            return Task.FromResult(Rot13Transform(input));
        }

        private string Rot13Transform(string input)
        {
            StringBuilder result = new StringBuilder(input.Length);

            foreach (char c in input)
            {
                if (c >= 'a' && c <= 'z')
                {
                    char shifted = (char)(((c - 'a' + 13) % 26) + 'a');
                    result.Append(shifted);
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    char shifted = (char)(((c - 'A' + 13) % 26) + 'A');
                    result.Append(shifted);
                }
                else
                {
                    //No alfabéticos se dejan igual
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        internal override string GetName()
        {
            return "ROT13";
        }
    }
}