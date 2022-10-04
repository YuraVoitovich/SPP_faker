using generator_interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegerGenerator
{
    public class IntGenerator : IGenerator
    {

        Random random = new Random();
        public IntGenerator() {
               
        }
        public object Generate()
        {
            return random.Next(int.MinValue, int.MaxValue);
        }
        
        public Type GetGeneratorType()
        {
            return typeof(int);
        }


    }
}
