using System;
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

        /**
         * Obtiene del vault tanto la private key como el IV, estos se guardan combinados por lo que
         * en su uso hay que separarlos con sus tamaños de 32 y 16 bytes. La primera vez que el usuario
         * acceda, se generará aleatoria esta cadena, el resto de las veces esta cadena se sacará del 
         * ordenador.
         * 
         * 
         */
        private async Task<(byte[] Key, byte[] IV)> GetOrCreateKeyAndIVAsync()
        {
            var vault = new PasswordVault();

            try
            {
                var credential = vault.Retrieve(vaultResource, vaultKeyName);
                credential.RetrievePassword();
                byte[] combined = Convert.FromBase64String(credential.Password);

                byte[] key = new byte[32]; // AES-256 que son 32 bytes
                byte[] iv = new byte[16];  // AES block size que es 16 bytes 

                
                Buffer.BlockCopy(combined, 0, key, 0, 32);
                Buffer.BlockCopy(combined, 32, iv, 0, 16);

                return (key, iv);
            }
            catch
            {
                byte[] key = System.Security.Cryptography.RandomNumberGenerator.GetBytes(32);
                byte[] iv = System.Security.Cryptography.RandomNumberGenerator.GetBytes(16);

                byte[] combined = new byte[48];
                Buffer.BlockCopy(key, 0, combined, 0, 32);
                Buffer.BlockCopy(iv, 0, combined, 32, 16);

                string encoded = Convert.ToBase64String(combined);
                vault.Add(new PasswordCredential(vaultResource, vaultKeyName, encoded));

                return (key, iv);
            }
        }

        protected override async Task<string> Decode(string input)
        {
            byte[] cipherText = Convert.FromBase64String(input);
            var (key, iv) = await GetOrCreateKeyAndIVAsync();

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
            var (key, iv) = await GetOrCreateKeyAndIVAsync();

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

        protected override string GetName()
        {
            return "Encriptación AES";
        }
    }
}