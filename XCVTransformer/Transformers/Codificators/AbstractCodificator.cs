﻿using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using XCVTransformer.AuxClasses;

namespace XCVTransformer.Transformers.Codificators
{
    /**
     * Se aplica aquí el patrón factory method, esta es la clase abstracta
     * que será el esqueleto de todos los codificadores concretos ya que comparten 
     * gran parte excepto la forma de codificar y decodificar como tal
     */
    abstract class AbstractCodificator : ITransformer, ICodificator
    {
        private bool codificatorMode = true;

        public void ChangeCodificatorMode(bool newMode)
        {
            this.codificatorMode = newMode;
        }

        public async Task<string> Transform(string toTransform)
        {
            try
            {
                if (codificatorMode)
                {
                    return await Encode(toTransform);
                }
                else
                {
                    return await Decode(toTransform);

                }
            }
            catch (FormatException)
            {
                NotificationLauncher.NotifyBadFormatForCodification(GetName());
            }
            catch (CryptographicException)
            {
                NotificationLauncher.NotifyCryptoError(GetName());
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error en el codificador", ex);
            }
            return "Error de codificación";
        }

        public (bool, string, int) MaxCharactersAllowed(int charactersNum)
        {
            if (charactersNum <= 500)
            {
                return (false, "Codificar", 500);
            }
            return (true, "Codificar", 500);
        }


        //---------------Métodos abstractos que las subclases deben implementar------------------
        internal abstract Task<string> Encode(string input);
        internal abstract Task<string> Decode(string input);
        internal abstract string GetName();
    }

}
