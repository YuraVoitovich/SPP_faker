using generator_interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
namespace IntegerGenerator
{
    public class IntegerGenerator<T> : IGenerator<T>
    {
        public T Generate()
        {
            throw new NotImplementedException();
           
        }

        public Type GetGeneratorType()
        {
            throw new NotImplementedException();
        }
    }
}
