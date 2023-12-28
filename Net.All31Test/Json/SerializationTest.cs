using Net.Proxy;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace Net.Json.Test
{
    public  class SerializationTest
    {
        [Fact]
        public void SerializeObjectTest()
        {
            var x = InterfaceType.NewProxy<IRequestInfo>();
            Expression<Func<IRequestInfo, bool>> exp = p => p.AppId == "123";
            var serialized=exp.Serialize();
            var desexp=serialized.Deserialize<Expression<Func<IRequestInfo, bool>>>();

            var obj = new
            {
                Property1 = "Property 1",
                Property2 = 2,
                _id = "3"
            };
            var result=obj.Serialize();
        }
        [Fact]
        public void CustomSerializeObjectTest()
        {
            var obj = new
            {
                Property1 = "Property 1",
                Property2 = 2,
                _id = "3"
            };
            var resolver = new DynamicContractResolver()
            {
                MongoIdConversion = false,
                LowerFirstLetter = false
            };
            SerializationExtensions.DefaultSettings.ContractResolver = resolver;
            var serializeText = obj.Serialize();
            var deserializeText= serializeText.Deserialize<ExpandoObject>();
        }
        [Fact]
        public void AsExpandoTest()
        {
            var obj = new
            {
                Property1 = "Property 1",
                Property2 = 2,
                _id = "3",
                List=new List<object>(new[]
                {
                    new
                    {
                        SubProp1="Sub Prop 1",
                        SubProp2=2,
                        _id="2"
                    }
                })
            };
            dynamic expando = obj.AsExpandoObject();
            var list = expando.list;
        }

        [Fact] 
        public void IntTest()
        {
            var x= "{'token':'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjlmYzM2NWRhLTlhNGYtNGVjMS1hYzczLTJkMmE2OTg3OWRkYiIsIm5hbWVpZCI6IjlmYzM2NWRhLTlhNGYtNGVjMS1hYzczLTJkMmE2OTg3OWRkYiIsImFpZCI6ImU2MmZlZDYxLTk5MTEtNDg3OS04YzQxLWExYzI3ZjU3ZGNkOCIsImNuYW1lIjoiU2FsaWggS2VsZcWfIiwiYXV0aCI6IjMiLCJqYWlkIjoiMDAwMDAwMDAtMDAwMC0wMDAwLTAwMDAtMDAwMDAwMDAwMDAwIiwicGljIjoiN2Y3OWE5NTAtYTcwYi00ZTNiLWIxYzUtNGVhMjZkOTAyYjgwIiwidW5hbWUiOiJzYWxpaGtlbGVzIiwidXNpZCI6IjlmYzM2NWRhLTlhNGYtNGVjMS1hYzczLTJkMmE2OTg3OWRkYiIsInVzYWNpZCI6IjAwMDAwMDAwLTAwMDAtMDAwMC0wMDAwLTAwMDAwMDAwMDAwMCIsImhvc3QiOiIiLCJpcCI6IjEwLjAuMzAuNDUiLCJhY3RpdmUtc3RhcnQtdGltZSI6IjAwOjAwOjAwIiwiYWN0aXZlLWVuZC10aW1lIjoiMS4wMDowMDowMCIsIm5iZiI6MTY3Nzg1OTk1NCwiZXhwIjoxNjc3ODYwNTU0LCJpYXQiOjE2Nzc4NTk5NTQsImlzcyI6ImJpLnRlY2hub2xvZ3kuYWNjb3VudC5zZXJ2aWNlIn0.MnTu00RocSUEEhOxcTmYZo4JfOiIcJfehpmLtF-laBE','authType':3,'language':'en','currency':'usd','iPAddress':'127.0.0.1','requestPath':'/api/application/application/info','endPoint':'custom:get:application/info','appId':'00000000-0000-0000-0000-000000000000','accountId':'e62fed61-9911-4879-8c41-a1c27f57dcd8','hotelId':'70073f67-b56a-41f0-8b8a-add7c679debc','force':false,'deviceId':'1669375995036-25779019339','rTokenExists':false,'tokenExists':true,'availableHotelIds':null,'availableCurrencies':null,'availableLanguages':null,'identity':{'name':'Salih Keleş','picture':'7f79a950-a70b-4e3b-b1c5-4ea26d902b80','accountId':null,'id':'9fc365da-9a4f-4ec1-ac73-2d2a69879ddb'},'userAccountId':'00000000-0000-0000-0000-000000000000','requestDate':'2023-03-03T16:21:41.2436831Z','requestDay':20230303,'timeOffset':10800000,'host':'admin.bitechnology.com','isAdministrator':false,'reservationId':null,'transactionId':'909GOlM4J0CInMaEC_7eHA','isAdminStaff':false,'referer':'http://admin.bitechnology.com/','isTest':false,'allLangs':false,'staffActiveTimeRange':{'start':0,'end':86400000},'tokenIPAddress':null,'tokenExpired':false}";
           var c= x.Deserialize<IRequestInfo>();
        }
    }
}
