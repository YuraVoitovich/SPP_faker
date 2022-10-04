using bool_generator;
using char_generator;
using collection_generator_interface;
using date_time_generator;
using double_generator;
using enumerable_generator;
using generator_interface;
using IntegerGenerator;
using Moq;
using string_generator;
using System.Collections;

namespace SPP_faker.Tests
{
    [TestClass]
    public class UnitTest1
    {

        private static IDictionary<Type, Mock<IGenerator>> generators;
        private static IDictionary<Type, Mock<ICollectionGenerator>> collectionGenerators;
        private static TestClassA generatedClass;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContexts)
        {

            generators = new Dictionary<Type, Mock<IGenerator>>();


            var generator = new Mock<IGenerator>();
            generator.Setup(o => o.Generate()).Returns('a');
            generators.Add(typeof(char), generator);

            generator = new Mock<IGenerator>();
            generator.Setup(o => o.Generate()).Returns("val");
            generators.Add(typeof(string), generator);

            generator = new Mock<IGenerator>();
            generator.Setup(o => o.Generate()).Returns(0.1);
            generators.Add(typeof(double), generator);

            generator = new Mock<IGenerator>();
            generator.Setup(o => o.Generate()).Returns(true);
            generators.Add(typeof(bool), generator);

            generator = new Mock<IGenerator>();
            generator.Setup(o => o.Generate()).Returns((short)32);
            generators.Add(typeof(short), generator);

            generator = new Mock<IGenerator>();
            generator.Setup(o => o.Generate()).Returns(new DateTime(1488, 12, 12));
            generators.Add(typeof(DateTime), generator);

            generator = new Mock<IGenerator>();
            generator.Setup(o => o.Generate()).Returns(25);
            generators.Add(typeof(int), generator);

            collectionGenerators = new Dictionary<Type, Mock<ICollectionGenerator>>();
            var collectionGenerator = new Mock<ICollectionGenerator>();

            collectionGenerator.Setup(o => o.generate(typeof(List<int>), generator.Object)).Returns(new List<int> { 1, 2, 3});
            collectionGenerators.Add(typeof(List<int>),collectionGenerator);

            LinkedList<int> testList = new LinkedList<int>();
            testList.AddLast(1);
            testList.AddLast(2);
            testList.AddLast(3);
            collectionGenerator.Setup(o => o.generate(typeof(LinkedList<int>), generator.Object)).Returns(testList);
            collectionGenerators.Add(typeof(IEnumerable<int>), collectionGenerator);

            FakerConfig config = new FakerConfig();
            config.TryFindInstanceMethod = true;
            IFaker faker = new Faker(config);

            Mock<IGenerator> generatorStub = null;
            Mock<ICollectionGenerator> collectionGeneratorStub = null;
            generators.TryGetValue(typeof(int), out generatorStub);
            faker.RegisterGenerator<int>(generatorStub.Object);

            generators.TryGetValue(typeof(double), out generatorStub);
            faker.RegisterGenerator<double>(generatorStub.Object);

            generators.TryGetValue(typeof(short), out generatorStub);
            faker.RegisterGenerator<short>(generatorStub.Object);

            generators.TryGetValue(typeof(char), out generatorStub);
            faker.RegisterGenerator<char>(generatorStub.Object);

            generators.TryGetValue(typeof(string), out generatorStub);
            faker.RegisterGenerator<string>(generatorStub.Object);

            generators.TryGetValue(typeof(DateTime), out generatorStub);
            faker.RegisterGenerator<DateTime>(generatorStub.Object);

            generators.TryGetValue(typeof(bool), out generatorStub);
            faker.RegisterGenerator<bool>(generatorStub.Object);
            


            collectionGenerators.TryGetValue(typeof(List<int>), out collectionGeneratorStub);
            faker.RegisterCollectionGenerator<List<int>>(collectionGeneratorStub.Object);

            collectionGenerators.TryGetValue(typeof(IEnumerable<int>), out collectionGeneratorStub);
            faker.RegisterCollectionGenerator<IEnumerable<int>>(collectionGeneratorStub.Object);


            generatedClass = faker.Create<TestClassA>();
        }

        [TestMethod]
        public void TestIntField()
        {
            Assert.AreEqual(25, generatedClass.intField);
        }

        [TestMethod]
        public void TestIntProperty()
        {
            Assert.AreEqual(25, generatedClass.intProperty);
        }

        [TestMethod]
        public void TestIntListField()
        {
            List<int> expectedList = new List<int> { 1, 2, 3 };
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(expectedList[i], generatedClass.list[i]);
            }            
        }

        [TestMethod]
        public void TestInLinkedtListField()
        {
            List<int> expectedList = new List<int> { 1, 2, 3 };
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(expectedList[i], generatedClass.linkedList.ToList()[i]);
            }
        }

        [TestMethod]
        public void TestCharField()
        {
            Assert.AreEqual('a', generatedClass.charField);
        }

        [TestMethod]
        public void TestStringField()
        {
            Assert.AreEqual("val", generatedClass.stringField);
        }

        [TestMethod]
        public void TestBoolField()
        {
            Assert.AreEqual(true, generatedClass.boolField);
        }

        [TestMethod]
        public void TestConstructorWithDTOAttribute()
        {
            Assert.IsNotNull(generatedClass.getTestB());
        }

        [TestMethod]
        public void TestInstanceMethod()
        {

            Assert.IsNotNull(generatedClass.getTestB());
        }



        [TestMethod]
        public void TestShortField()
        {
            Assert.AreEqual(32, generatedClass.shortField);
        }

        [TestMethod]
        public void TestDateTimeField()
        {
            Assert.AreEqual(new DateTime(1488, 12, 12), generatedClass.DateTimeField);
        }

        [TestMethod]
        public void TestDoubleField()
        {
            Assert.AreEqual(0.1, generatedClass.doubleField);
        }
    }
}