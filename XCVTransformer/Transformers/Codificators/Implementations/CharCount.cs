using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCVTransformer.Transformers.Codificators.Implementations
{
    class CharCount : AbstractCodificator
    {
        protected override Task<string> Encode(string input)
        {
            int count = input.Length;
            return Task.FromResult(count.ToString());
        }
        //Igual ambos
        protected override Task<string> Decode(string input)
        {
            int count = input.Length;
            return Task.FromResult(count.ToString());
        }

        protected override string GetName()
        {
            return "Número de caracteres";
        }
    }
}
