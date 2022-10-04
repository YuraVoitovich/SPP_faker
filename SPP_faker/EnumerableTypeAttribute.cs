using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPP_faker
{
    public class EnumerableTypeAttribute : Attribute
    {
        public Type Type { get; set; }
        
        public EnumerableTypeAttribute(Type type)
        {
            if (type.IsInterface)
            {
                throw new ArgumentException("Type can't be an interface");            
            }
            List<Type> interfaces = type.GetInterfaces().ToList();
            Type enumerableType = typeof(IEnumerable<>);
            Type collectionType = interfaces.Find(o => o.Name.Equals(enumerableType.Name));
           
            if (collectionType == null) {
                throw new ArgumentException("Type should implement IEnumerable<>");
            }
            this.Type = type;
        }
    }
}
