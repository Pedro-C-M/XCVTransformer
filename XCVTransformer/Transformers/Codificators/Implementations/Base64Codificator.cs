using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCVTransformer.AuxClasses;

namespace XCVTransformer.Transformers.Codificators.Implementations
{
    class Base64Codificator : AbstractCodificator
    {
        protected override Task<string> Decode(string input)
        {
            byte[] bytes = Convert.FromBase64String(input);
            return Task.FromResult(Encoding.UTF8.GetString(bytes));
        }

        protected override Task<string> Encode(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            return Task.FromResult(Convert.ToBase64String(bytes));
        }

        protected override string GetName()
        {
            return "Base64";
        }
    }
}
