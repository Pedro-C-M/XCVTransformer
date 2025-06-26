using System;
using System.Text;
using System.Threading.Tasks;

namespace XCVTransformer.Transformers.Codificators.Implementations
{
    class Base64Codificator : AbstractCodificator
    {
        internal override Task<string> Decode(string input)
        {
            byte[] bytes = Convert.FromBase64String(input);
            return Task.FromResult(Encoding.UTF8.GetString(bytes));
        }

        internal override Task<string> Encode(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            return Task.FromResult(Convert.ToBase64String(bytes));
        }

        internal override string GetName()
        {
            return "Base64";
        }
    }
}
