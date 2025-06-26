using System.Threading.Tasks;

namespace XCVTransformer.Transformers.Codificators
{
    interface IKeyStorage
    {
        Task<(byte[] Key, byte[] IV)> GetOrCreateKeyAndIVAsync();
    }
}
