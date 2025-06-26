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
        private readonly IKeyStorage keyStorage;

        public BlowfishCodificator(IKeyStorage keyStorage)
        {
            this.keyStorage = keyStorage;
        }

        internal override async Task<string> Encode(string input)
        {
            var (key, iv) = await keyStorage.GetOrCreateKeyAndIVAsync();

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

        internal override async Task<string> Decode(string input)
        {
            var (key, iv) = await keyStorage.GetOrCreateKeyAndIVAsync();

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

        internal override string GetName()
        {
            return "Encriptación Blowfish";
        }
    }
}
