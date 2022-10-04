using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using collection_generator_interface;
using generator_interface;
using generator_interface.exception;

namespace enumerable_generator
{
    public class EnumerableGenerator : ICollectionGenerator
    {
        public EnumerableGenerator()
        {
            HasDefaultRealization = false;
        }

        public EnumerableGenerator(Type defaultRealizationType, IGenerator defaultElementGenerator)
        {
            HasDefaultRealization = true;
            DefaultRealizationType = defaultRealizationType;
            DefaultElementGenerator = defaultElementGenerator;
        }

        private Random random = new Random();
        public bool HasDefaultRealization { private set;  get; }
   
        public Type DefaultRealizationType { private set; get; }

        public IGenerator DefaultElementGenerator { private set; get; }

        private bool isEnumerableType(Type type)
        {
            return type.GetInterfaces().Contains(typeof(IEnumerable<>));
        }
        public void SetDefaultRealization(Type collectionType, IGenerator elementGenerator)
        {
            if (!isEnumerableType(collectionType))
            {
                throw new ArgumentException("Not valid type");
            }
            DefaultRealizationType = collectionType;
            DefaultElementGenerator = elementGenerator;
            HasDefaultRealization = true;
        }
        public object generate(Type collectionType, IGenerator elementGenerator)
        {
            if (collectionType == null && HasDefaultRealization) 
                return generateCollection(DefaultRealizationType, elementGenerator, out var _size);
            return generateCollection(collectionType, elementGenerator, out var size); 
        }

        private object generateCollection(Type collectionType, IGenerator elementGenerator, out int size, bool fillElements = true)
        {
            object generatedCollection = collectionType.GetConstructor(new Type[0]).Invoke(null);
            var genericArguments = collectionType.GetGenericArguments();
            MethodInfo addMethod = typeof(ICollection<>).MakeGenericType(collectionType.GetGenericArguments()).GetMethod("Add");
            List<object> list = new List<object>();
            int collectionSize = random.Next(1, 15);
            if (fillElements)
            {
                for (int i = 0; i < collectionSize; i++)
                {
                    //addMethod = addMethod.MakeGenericMethod(collectionType.GetGenericArguments()[0]);

                    //var newMethod = addMethod.MakeGenericMethod(collectionType.GetGenericArguments()[0]);
                    addMethod.Invoke(generatedCollection, new object[] { elementGenerator.Generate() });
                }
            }
            size = collectionSize;
            return generatedCollection;
        }

        public object Generate()
        {
            if (!HasDefaultRealization)
            {
                throw new CollectionGeneratorException("Generator has not default generator");
            }
            return generateCollection(DefaultRealizationType, DefaultElementGenerator, out var size);
        }

        public Type GetGeneratorType()
        {
            if (HasDefaultRealization) return DefaultRealizationType;
            return typeof(IList<>);
        }

        public object generate(Type collectionType, out int size)
        {
            return generateCollection(collectionType, null, out size, false);
        }
    }
}
