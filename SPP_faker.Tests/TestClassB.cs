using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPP_faker.Tests
{

    [DTO]
    public class TestClassB
    {

        public int field { get; private set; }

        public static TestClassB INSTANCE(int field)
        {
            return new TestClassB(field);
        }

        private TestClassB(int filed)
        {
            this.field = field;
        } 
    }
}
