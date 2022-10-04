using generator_interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPP_faker.Tests
{
    internal class IntCustomGenerator : IGenerator
    {
        public object Generate()
        {
            return 1;
        }

        public Type GetGeneratorType()
        {
            return typeof(int);
        }
    }
}
