using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SPP_faker
{
    public class Faker : IFaker
    {
        private T UseParameterizedConstructor<T>(ConstructorInfo constructorInfo)
        {
            ParameterInfo[] parameterInfos = constructorInfo.GetParameters();
        }

        public T Create<T>()
        {
            Type type = typeof(T);
            object[] attributes = type.GetCustomAttributes(false);

            bool isValid = false;
            // проходим по всем атрибутам
            foreach (Attribute attr in attributes)
            {
                if (attr is DTOAttribute DTOAttribute)
                    isValid = true;
            }
            if (!isValid)
            {
                throw new Exception("Class should have an DTOAttribute");
            }
            ConstructorInfo[] constructorInfos =  type.GetConstructors();
            ConstructorInfo usedConstructor = null;
            foreach (ConstructorInfo constructorInfo in constructorInfos)
            {
                IEnumerable<Attribute> attrs = constructorInfo.GetCustomAttributes();
                foreach (Attribute attr in attrs)
                {
                    if (attr is DTOConstructorAttribute constructorAttribute)
                    {
                        usedConstructor = constructorInfo;
                    }
                }
            }
            bool isDefaultConstructor = false;
            if (usedConstructor == null)
            {
                ConstructorInfo constructor = type.GetConstructor(new Type[0]);
                if (constructor == null)
                {
                    throw new FakerException("Class should have constructor with DTOAttribute, or constructor without parameters");
                }
                usedConstructor = constructor;
                isDefaultConstructor = true;
            }

            T generatedDTO;
            if (!isDefaultConstructor)
            {
                ParameterInfo[] parameterInfos = usedConstructor.GetParameters();
                if (parameterInfos.Length == 0)
                {
                    generatedDTO = (T)usedConstructor.Invoke(null);
                } else
                {

                }
            }
        }
    }
}
