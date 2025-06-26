using System.Threading.Tasks;

namespace XCVTransformer.Transformers.Codificators
{
    public interface IKeyStorage
    {
        public Task<(byte[] Key, byte[] IV)> GetOrCreateKeyAndIVAsync();
    }
}
