using generator_interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace string_generator
{
    public class StringGenerator : IGenerator
    {
        private Random random = new Random();
        private const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-";

        public object Generate()
        {
            int length = random.Next(6, 20);
            char[] chars = new char[length];

            for (int i = 0; i < length; i++)
            {
                chars[i] = allowedChars[random.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }

        public Type GetGeneratorType()
        {
            return typeof(string);
        }
    }
}
