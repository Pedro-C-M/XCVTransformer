using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace XCVTransformer.Transformers.Codificators.Implementations
{
    /**
     * Similar a AES, en aes se encuentra la explicación
     * 
     */
    class DESCodificator : AbstractCodificator
    {
        private readonly IKeyStorage keyStorage;

        public DESCodificator(IKeyStorage keyStorage)
        {
            this.keyStorage = keyStorage;
        }

        internal override async Task<string> Encode(string input)
        {
            var (key, iv) = await keyStorage.GetOrCreateKeyAndIVAsync();

            using var des = DES.Create();
            des.Key = key;
            des.IV = iv;

            using var encryptor = des.CreateEncryptor();
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using var sw = new StreamWriter(cs);

            sw.Write(input);
            sw.Flush();
            cs.FlushFinalBlock();

            byte[] encrypted = ms.ToArray();
            return Convert.ToBase64String(encrypted);
        }

        internal override async Task<string> Decode(string input)
        {
            byte[] cipherText = Convert.FromBase64String(input);
            var (key, iv) = await keyStorage.GetOrCreateKeyAndIVAsync();

            using var des = DES.Create();
            des.Key = key;
            des.IV = iv;

            using var decryptor = des.CreateDecryptor();
            using var ms = new MemoryStream(cipherText);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            return sr.ReadToEnd();
        }

        internal override string GetName()
        {
            return "Encriptación DES";
        }


    }
}
