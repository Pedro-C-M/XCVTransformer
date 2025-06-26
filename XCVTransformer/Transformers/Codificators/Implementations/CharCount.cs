using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCVTransformer.Transformers.Codificators.Implementations
{
    class CharCount : AbstractCodificator
    {
        internal override Task<string> Encode(string input)
        {
            int count = input.Length;
            return Task.FromResult(count.ToString());
        }
        //Igual ambos
        internal override Task<string> Decode(string input)
        {
            int count = input.Length;
            return Task.FromResult(count.ToString());
        }

        internal override string GetName()
        {
            return "Número de caracteres";
        }
    }
}
