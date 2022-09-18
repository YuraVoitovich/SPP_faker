using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPP_faker
{
    public interface IFaker
    {
         T Create<T>(); 
    }
}
