using System;
using System.Linq;
using System.Threading.Tasks;

namespace XCVTransformer.Transformers.Codificators.Implementations
{
    class Inversor : AbstractCodificator
    {
        /**
         * En el caso de revertir el orden es lo mismo codificar que decodificar
         */
        internal override Task<string> Decode(string input)
        {
            return Task.FromResult(new string(input.Reverse().ToArray()));
        }

        internal override Task<string> Encode(string input)
        {
            return Task.FromResult(new string(input.Reverse().ToArray()));
        }

        internal override string GetName()
        {
            return "Invertir orden";
        }
    }
}
