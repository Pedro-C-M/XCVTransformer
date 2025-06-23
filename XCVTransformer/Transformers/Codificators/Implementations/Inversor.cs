using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCVTransformer.Transformers.Codificators.Implementations
{
    class Inversor : AbstractCodificator
    {
        /**
         * En el caso de revertir el orden es lo mismo codificar que decodificar
         */
        protected override Task<string> Decode(string input)
        {
            return Task.FromResult(new string(input.Reverse().ToArray()));
        }

        protected override Task<string> Encode(string input)
        {
            return Task.FromResult(new string(input.Reverse().ToArray()));
        }

        protected override string GetName()
        {
            return "Invertir orden";
        }
    }
}
