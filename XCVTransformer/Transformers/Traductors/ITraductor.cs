using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCVTransformer.Transformers
{
    public interface ITraductor : ITransformer
    {
        void ChangeOriginCode(string newCode);
        void ChangeEndCode(string newCode);
        bool SameFromTo();
    }
}
