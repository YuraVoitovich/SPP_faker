using generator_interface;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace collection_generator_interface
{
    public interface ICollectionGenerator : IGenerator
    {
        object generate(Type collectionType, IGenerator generator); 
        object generate(Type collectionType, out int size);
    }
}
