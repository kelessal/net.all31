using Net.Extensions;
using Newtonsoft.Json.Linq;
using System;
using Xunit;

namespace Net.Reflection.Test
{
    public class TypeExtensionsTest
    {
        [Fact]
        public void LogicalEqualTest()
        {
            var obj1 = new
            {
                Name = "hello",
                Address = new
                {
                    City = "İstanbul",
                    Countr = "Turkey"
                },
                Numbers =(int[]) null
            };
            var obj2 = new
            {
                Name = "hello",
                Address = new
                {
                    City = "İstanbul",
                    Countr = "Turkey"
                },
                Numbers = new[] { 1, 2, 3 }
            };
        }

        [Fact]
        public void ConvertToDictionaryTest()
        {
            var obj = new
            {
                Name = "hello",
                Address = new
                {
                    City = "İstanbul",
                    Countr = "Turkey"
                },
                Numbers=new[] {1,2,3}
            };
            obj.ConvertToExpando();
            var result= obj.ConvertToExpando();
        }
        [Fact]
        public void SetPathValueTest()
        {
            var item= new TestObject();
            item.SetPathValue("NTO.Index", -3);
            
        }
        [Fact]
        public void AsTest()
        {
            var item = new { name = "hello", age = "3" };

            var x=item.As<TestObject>();

        }
        [Fact]
        public void JsonTest()
        {
            var item = new { name = "hello", age = "3" };

            var x = item.As<JObject>();
            var result=x.GetPropValue("name");
        }
        [Fact]
        public void CamelCaseTest()
        {
            var item = new TestObject();
            var type=item.GetType().GetInfo();
            var prop = type.GetPropertyByPath("nto.propA");
        }
    }
}
