using System;
using System.Threading.Tasks;

namespace XCVTransformer.Transformers
{
    public interface ITransformer
    {
        Task<string> Transform(string toTransform);
    }
}
