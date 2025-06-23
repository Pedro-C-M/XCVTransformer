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
        private const string vaultResource = "XCVTransformer";
        private const string vaultKeyName = "DESKey";

        private async Task<(byte[] Key, byte[] IV)> GetOrCreateKeyAndIVAsync()
        {
            var vault = new PasswordVault();
            try
            {
                var credential = vault.Retrieve(vaultResource, vaultKeyName);
                credential.RetrievePassword();
                byte[] combined = Convert.FromBase64String(credential.Password);

                byte[] key = new byte[8]; // DES usa claves de 64 bits (8 bytes)
                byte[] iv = new byte[8];  // IV también de 8 bytes

                Buffer.BlockCopy(combined, 0, key, 0, 8);
                Buffer.BlockCopy(combined, 8, iv, 0, 8);

                return (key, iv);
            }
            catch
            {
                using var desTemp = DES.Create();
                byte[] key = desTemp.Key;
                byte[] iv = desTemp.IV;

                byte[] combined = new byte[16];
                Buffer.BlockCopy(key, 0, combined, 0, 8);
                Buffer.BlockCopy(iv, 0, combined, 8, 8);

                string encoded = Convert.ToBase64String(combined);
                vault.Add(new PasswordCredential(vaultResource, vaultKeyName, encoded));

                return (key, iv);
            }
        }

        protected override async Task<string> Encode(string input)
        {
            var (key, iv) = await GetOrCreateKeyAndIVAsync();

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

        protected override async Task<string> Decode(string input)
        {
            byte[] cipherText = Convert.FromBase64String(input);
            var (key, iv) = await GetOrCreateKeyAndIVAsync();

            using var des = DES.Create();
            des.Key = key;
            des.IV = iv;

            using var decryptor = des.CreateDecryptor();
            using var ms = new MemoryStream(cipherText);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            return sr.ReadToEnd();
        }

        protected override string GetName()
        {
            return "Encriptación DES";
        }


    }
}
