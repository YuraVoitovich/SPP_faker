using generator_interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegerGenerator
{
    public class ShortGenerator : IGenerator
    {
        private Random random = new Random();
        public object Generate()
        {
            return random.Next(short.MinValue, short.MaxValue);
        }

        public Type GetGeneratorType()
        {
            return typeof(short);
        }
    }
}
