using System;
using System.Text;
using System.Threading.Tasks;

namespace XCVTransformer.Transformers.Codificators.Implementations
{
    /**
     * Hexadecimal modificado para que las letras salgan mayúsculas y haya espacios cada par de caracteres hexadecimal
     */
    class HexCodificator : AbstractCodificator
    {
        protected override Task<string> Encode(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            StringBuilder hex = new StringBuilder(bytes.Length * 3);

            for (int i = 0; i < bytes.Length; i++)
            {
                hex.AppendFormat("{0:X2}", bytes[i]);
                if (i != bytes.Length - 1)
                    hex.Append(" ");
            }
            return Task.FromResult(hex.ToString());
        }

        protected override Task<string> Decode(string input)
        {
            string[] hexValues = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            byte[] bytes = new byte[hexValues.Length];

            for (int i = 0; i < hexValues.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexValues[i], 16);
            }
            return Task.FromResult(Encoding.UTF8.GetString(bytes));
        }

        protected override string GetName()
        {
            return "Hexadecimal";
        }
    }
}
