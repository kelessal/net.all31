using System;
using System.Dynamic;
using Xunit;

namespace Net.Json.Test
{
    public class IdTest
    {
        [Fact]
        public void Test1()
        {
            dynamic obj = new ExpandoObject();
            obj._id = "Hello";
            obj.NameList = "World";
            var result = SerializationExtensions.Serialize(obj);
        }

        [Fact]
        public void LoopTest()
        {
            var loop = new LoopTestObj();
            loop.LoopProp = loop;
            loop.Name = "test";
            var result = SerializationExtensions.Serialize(loop);
        }
        [Fact]
        public void TestCamelCase()
        {
            var obj = new
            {
                BOLGE_ADI="ANTALYA",
                CIKIS_TARIHI="123"
            };
            var result = SerializationExtensions.Serialize(obj);
            var r2 = SerializationExtensions.Deserialize(result,obj.GetType());
        }
        [Fact]
        public void TestArray()
        {
            var obj = new[] { 1, 2, 3 };
            var result = SerializationExtensions.Serialize(obj);
            var r2 = SerializationExtensions.Deserialize(result, obj.GetType());
        }

        [Fact]
        public void IndentSerialize()
        {
            var obj = new
            {
                _id = "Hello",
                GIRIS_TARIHI = "World"
            };
            var result = obj.Serialize(true);
        }
        [Fact]
        public void TestTimeSpan()
        {
            var x= new TimeSpan(23,59,0);
            var result2=x.Ticks;
            var obj1 = new
            {
                _id = "Hello",
                GIRIS_TARIHI =x
            };
          
            var result = obj1.Serialize(true);
            var deserialized = "{GIRIS_TARIHI:68760000}".Deserialize(obj1.GetType());
        }
        [Fact]
        public void TestDateTime()
        {
            var x = DateTime.UtcNow;

            var y = x.Serialize();
            var z = "1234323432432".Deserialize<DateTime>().ToUniversalTime();
            Assert.Equal(x.Ticks,z.Ticks);
        }

        [Fact]
        public void ExpandoTest()
        {
            var x = new[] { "anv" };

            var y = x.AsExpandoObject();
        }

    }
}
