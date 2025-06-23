using System.Threading.Tasks;

namespace XCVTransformer.Transformers
{
    public interface ITransformer
    {
        Task<string> Transform(string toTransform);
        (bool overMax, string accion, int limit) MaxCharactersAllowed(int charactersNum);//Esto para devolver una tupla pero de 3, me viene bien 
    }
}
