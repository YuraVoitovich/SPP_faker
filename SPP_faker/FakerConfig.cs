using generator_interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SPP_faker
{
    public class FakerConfig
    {
        private IDictionary<Type, Dictionary<MemberInfo, IGenerator>> customGenerators = new Dictionary<Type, Dictionary<MemberInfo, IGenerator>>();
        private IList<Exception> suppressedExeptions = new List<Exception>();

        public bool SuppressGeneratorExceptionsAndReturnNull { get; set; }

        public bool SuppressConstructExceptionsAndReturnNull { get; set; }

        public bool TryFindInstanceMethod { get; set; }

        public bool TryCreateWithoutDTOAttribute { get; set; }

        public FakerConfig()
        {
            this.SuppressConstructExceptionsAndReturnNull = false;
            this.SuppressGeneratorExceptionsAndReturnNull = false;
            this.TryFindInstanceMethod = false;
            this.TryCreateWithoutDTOAttribute = false;
        }

        public IList<Exception> GetSuppressedExeptions()
        {
            return suppressedExeptions;
        }

        public void AddSuppressedExeption(Exception e)
        {
            this.suppressedExeptions.Add(e);
        }

        public IDictionary<MemberInfo, IGenerator> GetCustomGeneratorsForType(Type type)
        {
            Dictionary<MemberInfo, IGenerator> result = null;
            Type t = customGenerators.Keys.ToList().Find(o => o.Name.Equals(type.Name));
            if (t != null) {
                customGenerators.TryGetValue(t, out result);
                return result;
            } else
            {
                return new Dictionary<MemberInfo, IGenerator>();
            }
        }

        public void add<T, FT>(Expression<Func<T, FT>> exp, IGenerator generator)
        {
            ParameterExpression param = (ParameterExpression)exp.Parameters[0];
            MemberExpression memberExpression = (MemberExpression)exp.Body;
            MemberInfo memberInfo = memberExpression.Member;
            Dictionary<MemberInfo, IGenerator> members = null;
            if (customGenerators.TryGetValue(typeof(T), out members))
            {
                members.Add(memberInfo, generator);
            } else
            {
                members = new Dictionary<MemberInfo, IGenerator>();
                members.Add(memberInfo, generator);
                customGenerators.Add(typeof(T), members);
            }
        }

        public FakerConfig(bool suppressGeneratorExceptionsAndReturnNull, bool suppressConstructExceptionsAndReturnNull, bool tryFindInstanceMethod, bool tryCreateWithoutDTOAttribute)
        { 
            SuppressGeneratorExceptionsAndReturnNull = suppressGeneratorExceptionsAndReturnNull;
            SuppressConstructExceptionsAndReturnNull = suppressConstructExceptionsAndReturnNull;
            TryFindInstanceMethod = tryFindInstanceMethod;
            TryCreateWithoutDTOAttribute = tryCreateWithoutDTOAttribute;
        }

    }
}
