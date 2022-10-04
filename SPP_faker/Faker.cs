using collection_generator_interface;
using generator_interface;
using generator_interface.exception;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SPP_faker
{
    public class Faker : IFaker
    {

        public FakerConfig config { private set; get; }
        private HashSet<Type> stack = new HashSet<Type>();
        private Dictionary<Type, ICollectionGenerator> collectionGenerators = new Dictionary<Type, ICollectionGenerator>();
        private Dictionary<Type, IGenerator> generators = new Dictionary<Type, IGenerator>();
        private LinkedList<Type> keySortedList = new LinkedList<Type>();
        public Faker(FakerConfig config)
        {
            this.config = config;
        }

        public Faker()
        {
            if (config == null)
            {
                config = new FakerConfig();
            }
        }

        private bool wasUsedInConstructor(string name, List<string> usedParameters)
        {
            return usedParameters.Find(o => o.Equals(name, StringComparison.OrdinalIgnoreCase)) != null;
        }
        private T SetPublicProperties<T>(T dto, Type type, List<string> usedParameters)
        {
            var customGenerators = config.GetCustomGeneratorsForType(type);
            foreach (PropertyInfo pi in dto.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!wasUsedInConstructor(pi.Name, usedParameters))
                {
                    if (pi.CanWrite)
                    {
                        PropertyInfo propertyInfo = (PropertyInfo)customGenerators.Keys.ToList().Find(o => o.Name.Equals(pi.Name));
                        if (propertyInfo != null)
                        {
                            customGenerators.TryGetValue(propertyInfo, out var customGenerator);
                            propertyInfo.SetValue(dto, customGenerator.Generate());
                        }
                        else
                        {
                            bool isValSetted = false;
                            foreach (Attribute attr in pi.GetCustomAttributes())
                            {
                                if (attr is EnumerableTypeAttribute enumerableTypeAttribute)
                                {
                                    try
                                    {
                                        pi.SetValue(dto, Create(enumerableTypeAttribute.Type));
                                    }
                                    catch (Exception e)
                                    {
                                        pi.SetValue(dto, GenerateExceptionOrReturnNull("Not valid Attribute argument " + enumerableTypeAttribute.Type.FullName + " for property with type " + pi.PropertyType.FullName, e));
                                    }
                                    isValSetted = true;
                                }
                            }
                            if (!isValSetted)
                                pi.SetValue(dto, Create(pi.PropertyType));
                        }
                    }
                }
            }
            return dto;
        }

        private T SetPublicFields<T>(T dto, Type type, List<string> usedParameters)
        {
            var customGenerators = config.GetCustomGeneratorsForType(type);
            foreach (FieldInfo fi in dto.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!wasUsedInConstructor(fi.Name, usedParameters))
                {
                    FieldInfo fieldInfo = (FieldInfo)customGenerators.Keys.ToList().Find(o => o.Name.Equals(fi.Name));
                    if (fieldInfo != null)
                    {
                        customGenerators.TryGetValue(fieldInfo, out var customGenerator);
                        fieldInfo.SetValue(dto, customGenerator.Generate());
                    }
                    else
                    {
                        bool isValSetted = false;
                        foreach (Attribute attr in fi.GetCustomAttributes())
                        {
                            if (attr is EnumerableTypeAttribute enumerableTypeAttribute)
                            {
                                try
                                {
                                    fi.SetValue(dto, Create(enumerableTypeAttribute.Type));
                                }
                                catch (Exception e)
                                {
                                    fi.SetValue(dto, GenerateExceptionOrReturnNull("Not valid Attribute argument " + enumerableTypeAttribute.Type.FullName + " for field with type " + fi.FieldType.FullName, e));
                                }
                                isValSetted = true;
                            }
                        }
                        if (!isValSetted)
                            fi.SetValue(dto, Create(fi.FieldType));
                    }
                }
            }
            return dto;
        }

        private ConstructorInfo GetDtoConstructConstructor(ConstructorInfo[] constructorInfos)
        {
            ConstructorInfo usedConstructor = null;
            usedConstructor = constructorInfos.ToList()
                .Find(o => o.GetCustomAttributes().ToList()
                .Find(f => f is DTOConstructorAttribute c) != null);
            return usedConstructor;
        }

        private ConstructorInfo GetDefaultConstructor(Type type)
        {
            ConstructorInfo defaultConstructor = null;
            ConstructorInfo constructor = type.GetConstructor(new Type[0]);
            if (constructor == null)
            {
                throw new FakerException("Type " + type.FullName + " should have constructor with DTOConstructorAttribute, or noArgs contructor");
            }
            defaultConstructor = constructor;
            return defaultConstructor;
        }

        private bool GetCollectionGenerator(Type type, out ICollectionGenerator collectionGenerator)
        {
            List<Type> interfaces = type.GetInterfaces().ToList();
            interfaces.Add(type);
            Type matchedType = keySortedList.ToList().Find(o => interfaces.Find(val => val.Name.Equals(o.Name)) != null);
            if (matchedType == null)
            {
                collectionGenerator = null;
                return false;
            } 
            bool result = collectionGenerators.TryGetValue(matchedType, out var generator);
            collectionGenerator = generator;
            return result;
        }

        private object GenerateExceptionOrReturnNull(string message, Exception innerException = null)
        {
            if (innerException is GeneratorException)
            {
                if (config.SuppressGeneratorExceptionsAndReturnNull)
                {
                    return null;
                } else
                {   
                    FakerException e = new FakerException(message, innerException);
                    config.AddSuppressedExeption(e);
                    throw e;
                }
            }
            if (config.SuppressConstructExceptionsAndReturnNull)
            {
                return null;
            }
            FakerException fe = new FakerException(message, innerException);
            config.AddSuppressedExeption(fe);
            throw fe;
        }

        private object GenerateValue(Type type)
        {
            if (generators.TryGetValue(type, out var generator))
            {
                return generator.Generate();
            }
            else
            {
                if (GetCollectionGenerator(type, out var collectionGenerator))
                {
                    if (generators.TryGetValue(type.GetGenericArguments()[0], out var elementGenerator))
                    {
                        return collectionGenerator.generate(type, elementGenerator);
                    } else
                    {
                        if (hasDTOAttribute(type.GetGenericArguments()[0]))
                        {
                            object emptyCollection = collectionGenerator.generate(type, out var size);
                            MethodInfo addMethod = typeof(ICollection<>).MakeGenericType(type.GetGenericArguments()).GetMethod("Add");
                            for (int i = 0; i < size; i++)
                            {
                                addMethod.Invoke(emptyCollection, new object[] { Create(type.GetGenericArguments()[0]) });

                            }
                            return emptyCollection;
                        }
                        else
                        {
                            return GenerateExceptionOrReturnNull("Construct faild", new CollectionGeneratorException("Collection element type hasn't got any generator"));
                        }
                    }
                }
                else
                {
                    return GenerateExceptionOrReturnNull("Construct faild", new GeneratorException("Cannot find generator for " + type.FullName));
                }
            }
        }

        private object findInstanceMethod(Type type, out List<string> usedParameters)
        {
            List<MethodInfo> methodInfos = type.GetMethods(BindingFlags.Static | BindingFlags.Public).ToList();
            var InstanceMethod = methodInfos.Find(o => o.Name.Equals("INSTANCE", StringComparison.OrdinalIgnoreCase));
            if (InstanceMethod == null)
            {
                throw new FakerException("Unable to find instance method");
            }
            ParameterInfo[] parameterInfos = InstanceMethod.GetParameters();
            object[] parameters = new object[parameterInfos.Length];

            List<string> up = new List<string>();
            var customGenerators = config.GetCustomGeneratorsForType(type);
            for (int i = 0; i < parameterInfos.Length; i++)
            {

                MemberInfo memberInfo = customGenerators.Keys.ToList().Find(o => o.Name.Equals(parameterInfos[i].Name, StringComparison.OrdinalIgnoreCase));
                if (memberInfo != null)
                {
                    customGenerators.TryGetValue(memberInfo, out var generator);
                    parameters[i] = generator.Generate();
                }
                else
                {
                    parameters[i] = Create(parameterInfos[i].ParameterType);
                }
                up.Add(parameterInfos[i].Name);
                
            }
            usedParameters = up;
            return InstanceMethod.Invoke(null, parameters);
        }

        private object Create(Type type)
        {
            if (hasCycleReferences(type))
            { 
                return GenerateExceptionOrReturnNull("Cycle reference with type " + type.FullName);
            }

            if (!hasDTOAttribute(type))
            {
                return GenerateValue(type);
            }

            stack.Add(type);

            ConstructorInfo[] constructorInfos = type.GetConstructors();
            ConstructorInfo dtoConstructConstructor = GetDtoConstructConstructor(constructorInfos);
            

            bool isDefaultConstructor = false;

            Object generatedDTO = null;
            List<string> usedParameters = null;
            if (dtoConstructConstructor == null)
            {
                isDefaultConstructor = true;
                try
                {
                    dtoConstructConstructor = GetDefaultConstructor(type);
                } catch (FakerException e)
                {
                    if (config.TryFindInstanceMethod)
                    {
                        try
                        {
                            generatedDTO = findInstanceMethod(type, out usedParameters);
                        }
                        catch (FakerException fe)
                        {
                            return GenerateExceptionOrReturnNull("Construct faild", fe);
                        }
                    }
                    else
                    {
                        return GenerateExceptionOrReturnNull("Construct faild", e);
                    }
                }
            }


            if (generatedDTO == null)
            {
                if (!isDefaultConstructor)
                {
                    ParameterInfo[] parameterInfos = dtoConstructConstructor.GetParameters();
                    if (parameterInfos.Length == 0)
                    {
                        generatedDTO = dtoConstructConstructor.Invoke(null);
                        usedParameters = new List<string>();
                    }
                    else
                    {
                        generatedDTO = UseParameterizedConstructor(type, dtoConstructConstructor, out usedParameters);
                    }
                }
                else
                {
                    generatedDTO = dtoConstructConstructor.Invoke(null);
                    usedParameters = new List<string>();
                }
            }
            generatedDTO = SetPublicFields(generatedDTO, type, usedParameters);
            generatedDTO = SetPublicProperties(generatedDTO, type, usedParameters);
            stack.Remove(type);
            return generatedDTO;
        }

        private bool hasDTOAttribute(Type type)
        {
            object[] attributes = type.GetCustomAttributes(false);
            bool isValid = false;
            // проходим по всем атрибутам

            foreach (Attribute attr in attributes)
            {
                if (attr is DTOAttribute DTOAttribute)
                    isValid = true;
            }
            if (config.TryCreateWithoutDTOAttribute)
            {
                if (!generators.TryGetValue(type, out var generator) 
                    && !GetCollectionGenerator(type, out var collectionGenerator))
                {
                    isValid = true;
                }
            }
            return isValid;
        }
        private Object UseParameterizedConstructor(Type type, ConstructorInfo constructorInfo, out List<string> usedParameters)
        {
            ParameterInfo[] parameterInfos = constructorInfo.GetParameters();
            object[] parameters = new object[parameterInfos.Length];
            
            List<string> up = new List<string>();
            Type t = up.GetType();
            var customGenerators = config.GetCustomGeneratorsForType(type);
            for (int i = 0; i < parameterInfos.Length; i++)
            {
                MemberInfo memberInfo = customGenerators.Keys.ToList().Find(o => o.Name.Equals(parameterInfos[i].Name, StringComparison.OrdinalIgnoreCase));
 
                if (memberInfo != null)
                {
                    parameters[i] = Create(memberInfo.DeclaringType);
                } else {
                    parameters[i] = Create(parameterInfos[i].ParameterType);
                }
                up.Add(parameterInfos[i].Name);
                           
            }
            usedParameters = up;
            return constructorInfo.Invoke(parameters);
        }

        private bool hasCycleReferences(Type type)
        {
            return stack.Contains(type); 
        }

        public T Create<T>()
        {
            Type type = typeof(T);

            return (T)Create(type);
        }

        public void RegisterCollectionGenerator<T>(ICollectionGenerator generator)
        {
            Type type = typeof(T);
            collectionGenerators.Add(typeof(T), generator);
            var node = keySortedList.Last;
            bool isAdded = false;
            
            while (node != null && !isAdded)
            {
                if (node.Value.GetInterfaces().ToList().Find(o => o.Name.Equals(type.Name)) != null)
                {
                    keySortedList.AddAfter(node, type);
                    isAdded = true;
                }
                else
                {
                    node = node.Previous;
                }
            }
            if (!isAdded)
            {
                keySortedList.AddFirst(type);
            }
        }
        public void RegisterGenerator<T>(IGenerator generator)
        {
            generators.Add(typeof(T), generator);
        }
    }
}
