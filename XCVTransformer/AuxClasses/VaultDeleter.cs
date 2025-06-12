using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace XCVTransformer.AuxClasses
{
    /**
     * Cuando hay que borrar una cadena del vault usar este método
     */
    static class VaultDeleter
    {
        private const string vaultResource = "XCVTransformer";


        public static void DeleteKey(string vaultKeyName)
        {
            try
            {
                var vault = new PasswordVault();
                var credential = vault.Retrieve(vaultResource, vaultKeyName);
                vault.Remove(credential);

                System.Diagnostics.Debug.WriteLine("Clave "+ vaultKeyName +"eliminada del PasswordVault.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"No se pudo eliminar la clave: {ex.Message}");
            }
        }
    }
}
