using System;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Modes;

namespace XCVTransformer.Transformers.Codificators.Implementations
{
    /**
     * Similar a AES, en aes se encuentra la explicación
     * 
     */
    class BlowfishCodificator : AbstractCodificator
    {
        private const string vaultResource = "XCVTransformer";
        private const string vaultKeyName = "BlowfishKey";

        private async Task<(byte[] Key, byte[] IV)> GetOrCreateKeyAndIVAsync()
        {
            var vault = new PasswordVault();

            try
            {
                var credential = vault.Retrieve(vaultResource, vaultKeyName);
                credential.RetrievePassword();
                byte[] combined = Convert.FromBase64String(credential.Password);

                byte[] key = new byte[16];
                byte[] iv = new byte[8];

                Buffer.BlockCopy(combined, 0, key, 0, 16);
                Buffer.BlockCopy(combined, 16, iv, 0, 8);

                return (key, iv);
            }
            catch
            {
                byte[] key = System.Security.Cryptography.RandomNumberGenerator.GetBytes(16);
                byte[] iv = System.Security.Cryptography.RandomNumberGenerator.GetBytes(8);

                byte[] combined = new byte[24];
                Buffer.BlockCopy(key, 0, combined, 0, 16);
                Buffer.BlockCopy(iv, 0, combined, 16, 8);

                string encoded = Convert.ToBase64String(combined);
                vault.Add(new PasswordCredential(vaultResource, vaultKeyName, encoded));

                return (key, iv);
            }
        }

        protected override async Task<string> Encode(string input)
        {
            var (key, iv) = await GetOrCreateKeyAndIVAsync();

            BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new BlowfishEngine()));
            cipher.Init(true, new ParametersWithIV(new KeyParameter(key), iv));

            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] outputBytes = new byte[cipher.GetOutputSize(inputBytes.Length)];

            int len = cipher.ProcessBytes(inputBytes, 0, inputBytes.Length, outputBytes, 0);
            len += cipher.DoFinal(outputBytes, len);

            byte[] final = new byte[len];
            Buffer.BlockCopy(outputBytes, 0, final, 0, len);

            return Convert.ToBase64String(final);
        }

        protected override async Task<string> Decode(string input)
        {
            var (key, iv) = await GetOrCreateKeyAndIVAsync();

            BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new BlowfishEngine()));
            cipher.Init(false, new ParametersWithIV(new KeyParameter(key), iv));

            byte[] inputBytes = Convert.FromBase64String(input);
            byte[] outputBytes = new byte[cipher.GetOutputSize(inputBytes.Length)];

            int len = cipher.ProcessBytes(inputBytes, 0, inputBytes.Length, outputBytes, 0);
            len += cipher.DoFinal(outputBytes, len);

            byte[] final = new byte[len];
            Buffer.BlockCopy(outputBytes, 0, final, 0, len);

            return System.Text.Encoding.UTF8.GetString(final);
        }

        protected override string GetName()
        {
            return "Encriptación Blowfish";
        }
    }
}
