using generator_interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace char_generator
{
    public class CharGenerator : IGenerator
    {
        private Random random = new Random();
        public object Generate()
        {
            return (char)random.Next(-1, 256);
        }

        public Type GetGeneratorType()
        {
            return typeof(char);
        }
    }
}
