using generator_interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace double_generator
{
    public class FloatGenerator : IGenerator
    {
        private Random random = new Random();
        public object Generate()
        {
            double x = random.NextDouble();
            return x * float.MinValue + (1 - x) * float.MaxValue;
        }

        public Type GetGeneratorType()
        {
            return typeof(float);
        }
    }
}
