using System;
using XCVTransformer.AuxClasses;
using System.Threading.Tasks;
using Windows.Security.Credentials;


namespace XCVTransformer.Transformers.Codificators.Implementations
{
    class AESCodificator : AbstractCodificator
    {
        /**
         * ESTO UTILIZA VA A USAR PASSWORD VAULT DE WINDOWS
         * 
         * Una clase proporcionada por Windows por defecto en sus paquetes que funciona
         * como un almacen seguro. Puede guardar cifrado y protegido por el OS cadenas de un usuario de la máquina.
         * 
         */
        private const string vaultResource = "XCVTransformer";
        private const string vaultKeyName = "AESKey";

        private async Task<byte[]> GetOrCreateKeyAsync()
        {
            var vault = new PasswordVault();

            try
            {
                var credential = vault.Retrieve(vaultResource, vaultKeyName);
                credential.RetrievePassword();
                return Convert.FromBase64String(credential.Password);
            }
            catch
            {
                // Si no existe, la generamos
                byte[] key = System.Security.Cryptography.RandomNumberGenerator.GetBytes(32); // AES-256 = 32 bytes
                string encodedKey = Convert.ToBase64String(key);

                vault.Add(new PasswordCredential(vaultResource, vaultKeyName, encodedKey));
                return key;
            }
        }

        protected override async Task<string> Decode(string input)
        {
            byte[] combined = Convert.FromBase64String(input);
            byte[] key = await GetOrCreateKeyAsync();

            byte[] iv = new byte[16];
            byte[] cipherText = new byte[combined.Length - 16];

            Buffer.BlockCopy(combined, 0, iv, 0, 16);
            Buffer.BlockCopy(combined, 16, cipherText, 0, cipherText.Length);

            using var aes = System.Security.Cryptography.Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            using var ms = new System.IO.MemoryStream(cipherText);
            using var cs = new System.Security.Cryptography.CryptoStream(ms, decryptor, System.Security.Cryptography.CryptoStreamMode.Read);
            using var sr = new System.IO.StreamReader(cs);

            return sr.ReadToEnd();
        }

        protected override async Task<string> Encode(string input)
        {
            byte[] key = await GetOrCreateKeyAsync();
            byte[] iv = System.Security.Cryptography.RandomNumberGenerator.GetBytes(16); // AES IV: 16 bytes

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

            // Devolvemos IV + Encrypted como base64
            byte[] combined = new byte[iv.Length + encrypted.Length];
            Buffer.BlockCopy(iv, 0, combined, 0, iv.Length);
            Buffer.BlockCopy(encrypted, 0, combined, iv.Length, encrypted.Length);

            return Convert.ToBase64String(combined);
        }

        protected override string GetName()
        {
            return "Encriptación AES";
        }
    }
}
