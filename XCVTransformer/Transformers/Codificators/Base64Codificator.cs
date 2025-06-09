using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCVTransformer.AuxClasses;

namespace XCVTransformer.Transformers.Codificators
{
    class Base64Codificator : ITransformer, ICodificator
    {
        //Variable que indica si estamos en modo codificar To (True) o From (False) 
        private bool codificatorMode = true;
        public void ChangeCodificatorMode(bool newMode)
        {
            this.codificatorMode = newMode;
        }

        /**
         * Simplemente se usa la clase nativa de C# de la clase System.Convert
         * 
         * Hay que devolver como task aunque sea sincrono la transformación
         */
        public Task<string> Transform(string toTransform)
        {
            try { 
             if (codificatorMode)
                {//Modo To
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(toTransform);
                    return Task.FromResult(Convert.ToBase64String(bytes));
                }
            else
                {//Modo From
                    byte[] bytes = Convert.FromBase64String(toTransform);
                    return Task.FromResult(System.Text.Encoding.UTF8.GetString(bytes));
                }
            }
            catch(FormatException ex)
            {
                NotificationLauncher.NotifyBadFormatForCodification("Base64");
            }
            catch(Exception e)
            {
                throw new ArgumentException("Error en el codificador", e);

            }
            return Task.FromResult("Error de codificación");
        }
    }
}
