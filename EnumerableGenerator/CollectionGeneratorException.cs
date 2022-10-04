using generator_interface.exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace collection_generator_interface
{
    public class CollectionGeneratorException : GeneratorException
    {


        public CollectionGeneratorException()
        {
        }

        public CollectionGeneratorException(string message) : base(message)
        {
        }

        public CollectionGeneratorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
