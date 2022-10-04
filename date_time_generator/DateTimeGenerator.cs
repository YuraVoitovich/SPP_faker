using generator_interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace date_time_generator
{
    public class DateTimeGenerator : IGenerator
    {
        private Random gen = new Random();
        DateTime start = new DateTime(1995, 1, 1);
        public object Generate()
        {

            int range = (DateTime.Today - start).Days;
            return start.AddDays(gen.Next(range));
        }

        public Type GetGeneratorType()
        {
            return typeof(DateTime);
        }
    }
}
