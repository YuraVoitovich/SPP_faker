using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPP_faker.Tests
{

    [DTO]
    public class TestClassA
    {
        // public TestClassB b { get; set; }
        [EnumerableType(typeof(LinkedList<int>))]
        public IEnumerable<int> linkedList;

        public List<int> list;

        public int intField;

        public int intProperty { get; set; }

        public double doubleField;

        public char charField;

        public short shortField;

        public string stringField;

        public DateTime DateTimeField;

        public bool boolField;

        //[EnumerableType(typeof(LinkedList<TestClassB>))]
        //public IEnumerable<TestClassB> linkedList;

        //public int val { get; private set; }

        //public char ch;

        //public bool boolVal;
        //public DateTime dateTime { get; set; }
        //public string str { get; set; }

        //public double doubleValue;

        //public TestClassB testClassB1;

        //private TestClassB testClassB2;
        //public static TestClassA Instance(int val, TestClassB b)
        //{
        //    return new TestClassA(val, b);
        //}
        //private TestClassA(int val, TestClassB b)
        //{
        //    //this.val = val;
        //    //this.testClassB2 = b;
        //}

        private TestClassB TestClassB;

        public TestClassB getTestB()
        {
            return this.TestClassB;
        }

        [DTOConstructor]
        public TestClassA(TestClassB b)
        {
            this.TestClassB = b;
        }
        
    }
}
