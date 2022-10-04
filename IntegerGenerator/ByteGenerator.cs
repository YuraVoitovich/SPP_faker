using generator_interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegerGenerator
{
    public class ByteGenerator : IGenerator
    {
        private Random random = new Random();
        public object Generate()
        {

            return random.Next(byte.MinValue, byte.MaxValue);
        }
        public Type GetGeneratorType()
        {
            return typeof(byte);
        }
    }
}
