using System;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace XCVTransformer.Transformers.Codificators
{
    class VaultKeyStorage : IKeyStorage
    {
        /**
         * ESTO UTILIZA VA A USAR PASSWORD VAULT DE WINDOWS
         * 
         * Una clase proporcionada por Windows por defecto en sus paquetes que funciona
         * como un almacen seguro. Puede guardar cifrado y protegido por el OS cadenas de un usuario de la máquina.
         * 
         */
        private const string vaultResource = "XCVTransformer";
        private String vaultKeyName;
        // Tamaños configurables
        private readonly int keySize;
        private readonly int ivSize;

        public VaultKeyStorage(string vaultKeyName, int keySize, int ivSize)
        {
            this.vaultKeyName = vaultKeyName;
            this.keySize = keySize;
            this.ivSize = ivSize;
        }

        /**
         * Obtiene del vault tanto la private key como el IV, estos se guardan combinados por lo que
         * en su uso hay que separarlos con sus tamaños de 32 y 16 bytes. La primera vez que el usuario
         * acceda, se generará aleatoria esta cadena, el resto de las veces esta cadena se sacará del 
         * ordenador.
         * 
         */
        public Task<(byte[] Key, byte[] IV)> GetOrCreateKeyAndIVAsync()
        {
            var vault = new PasswordVault();

            try
            {
                var credential = vault.Retrieve(vaultResource, vaultKeyName);
                credential.RetrievePassword();
                byte[] combined = Convert.FromBase64String(credential.Password);

                byte[] key = new byte[keySize];
                byte[] iv = new byte[ivSize];

                Buffer.BlockCopy(combined, 0, key, 0, keySize);
                Buffer.BlockCopy(combined, keySize, iv, 0, ivSize);

                return Task.FromResult((key, iv));
            }
            catch
            {
                byte[] key = System.Security.Cryptography.RandomNumberGenerator.GetBytes(keySize);
                byte[] iv = System.Security.Cryptography.RandomNumberGenerator.GetBytes(ivSize);

                byte[] combined = new byte[keySize + ivSize];
                Buffer.BlockCopy(key, 0, combined, 0, keySize);
                Buffer.BlockCopy(iv, 0, combined, keySize, ivSize);

                string encoded = Convert.ToBase64String(combined);
                vault.Add(new PasswordCredential(vaultResource, vaultKeyName, encoded));

                return Task.FromResult((key, iv));
            }
        }
    }
}
