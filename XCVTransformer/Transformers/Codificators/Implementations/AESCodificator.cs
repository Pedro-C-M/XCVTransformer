using System;
using XCVTransformer.AuxClasses;
using System.Threading.Tasks;
using Windows.Security.Credentials;


namespace XCVTransformer.Transformers.Codificators.Implementations
{
    class AESCodificator : ITransformer, ICodificator
    {
        //Variable que indica si estamos en modo codificar To (True) o From (False) 
        private bool codificatorMode = true;
        public void ChangeCodificatorMode(bool newMode)
        {
            codificatorMode = newMode;
        }

        /**
         * Simplemente se usa la clase nativa de C# de la clase System.Convert
         * 
         * Hay que devolver como task aunque sea sincrono la transformación
         */
        public Task<string> Transform(string toTransform)
        {
            try
            {
                if (codificatorMode)
                {//Modo To
                    
                }
                else
                {//Modo From
                    
                }
            }
            catch (FormatException ex)
            {
                NotificationLauncher.NotifyBadFormatForCodification("Base64");
            }
            catch (Exception e)
            {
                throw new ArgumentException("Error en el codificador", e);

            }
            return Task.FromResult("Error de codificación");
        }
    }
}
