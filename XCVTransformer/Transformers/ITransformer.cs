using System.Threading.Tasks;

namespace XCVTransformer.Transformers
{
    public interface ITransformer
    {
        Task<string> Transform(string toTransform);
        void ChangeOriginCode(string newCode);

        void ChangeEndCode(string newCode);
    }
}
