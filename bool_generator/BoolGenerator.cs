using generator_interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bool_generator
{
    public class BoolGenerator : IGenerator
    {
        private Random random = new Random();
        public object Generate()
        {
            if (random.Next(-1, 2) == 1)
                return true;
            return false;
        }

        public Type GetGeneratorType()
        {
            return typeof(bool);
        }
    }
}
