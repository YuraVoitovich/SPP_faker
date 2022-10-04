using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace generator_interface.exception
{
    public class GeneratorException : Exception
    {
        public GeneratorException()
        {
        }

        public GeneratorException(string message) : base(message)
        {
        }

        public GeneratorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        
    }
}
