using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCVTransformer.Transformers.Codificators
{
    public interface ICodificator : ITransformer
    {
        void ChangeCodificatorMode(bool newMode);
    }
}
