using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPP_faker
{
    public class FakerException : Exception
    {
        public FakerException(string message) : base(message)
        {

        }

        public FakerException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
