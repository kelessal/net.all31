using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Reflection.Test
{
    class TestObject
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public NestedTestObject NTO { get; private set; } = new NestedTestObject();
    }
    class NestedTestObject
    {
        public string PropA { get; set; }
        public int? Index { get; set; }
    }
}
