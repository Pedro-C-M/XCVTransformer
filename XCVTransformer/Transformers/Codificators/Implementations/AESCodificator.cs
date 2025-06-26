using System;
using System.Threading.Tasks;
using Windows.Security.Credentials;


namespace XCVTransformer.Transformers.Codificators.Implementations
{
    class AESCodificator : AbstractCodificator
    {
        private readonly IKeyStorage keyStorage;

        public AESCodificator(IKeyStorage keyStorage)
        {
            this.keyStorage = keyStorage;
        }
        internal override async Task<string> Decode(string input)
        {
            byte[] cipherText = Convert.FromBase64String(input);
            var (key, iv) = await keyStorage.GetOrCreateKeyAndIVAsync();

            using var aes = System.Security.Cryptography.Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            using var ms = new System.IO.MemoryStream(cipherText);
            using var cs = new System.Security.Cryptography.CryptoStream(ms, decryptor, System.Security.Cryptography.CryptoStreamMode.Read);
            using var sr = new System.IO.StreamReader(cs);

            return sr.ReadToEnd();
        }

        internal override async Task<string> Encode(string input)
        {
            var (key, iv) = await keyStorage.GetOrCreateKeyAndIVAsync();

            using var aes = System.Security.Cryptography.Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using var encryptor = aes.CreateEncryptor();
            using var ms = new System.IO.MemoryStream();
            using var cs = new System.Security.Cryptography.CryptoStream(ms, encryptor, System.Security.Cryptography.CryptoStreamMode.Write);
            using var sw = new System.IO.StreamWriter(cs);

            sw.Write(input);
            sw.Flush();
            cs.FlushFinalBlock();

            byte[] encrypted = ms.ToArray();
            return Convert.ToBase64String(encrypted); 
        }

        internal override string GetName()
        {
            return "Encriptación AES";
        }
    }
}