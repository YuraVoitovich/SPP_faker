using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace generator_interface
{
    public interface IGenerator<T>
    {
        Type GetGeneratorType();

        void RegisterGenerator(IGenerator<T> generator);
        T Generate();
    }
}
