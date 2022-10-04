using generator_interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace double_generator
{
    public class DoubleGenerator : IGenerator
    {

        private Random random = new Random();
        public object Generate()
        {
            double x = random.NextDouble();
            return x * double.MinValue + (1 - x) * double.MaxValue;
        }

        public Type GetGeneratorType()
        {
            return typeof(double);
        }
    }
}
