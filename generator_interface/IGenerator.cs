using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace generator_interface
{
    public interface IGenerator
    {
        Type GetGeneratorType();

        void RegisterGenerator(IGenerator generator);
        T Generate<T>();
    }
}
