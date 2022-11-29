using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _11_2
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    class ValidateInt32Attribute : Attribute
    {     
        public int MinValue { get; set; }

        public int MaxValue { get; set; }

        public bool ZeroEnabled { get; set; }

        public ValidateInt32Attribute(int minValue, int maxValue, bool zeroEnabled)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            ZeroEnabled = zeroEnabled;
        }
    }

    class InvalidValueException : Exception
    {
        public int FieldName { get; set; }

        public int ListOfRestrictions { get; set; }

        public int CurrentValue { get; set; }

        public string ExeptionName { get; set; }

    }

    static class Int32Validate
    {
        public static void Validate(object obj)
        {
            PropertyInfo[] propList = obj.GetType().GetProperties();
            FieldInfo[] fieldList = obj.GetType().GetFields();
        }
    }

    class MyType
    {
        [ValidateInt32(1, 5, true)]
        int field1 = 5;
        [ValidateInt32(5, 32, false)]
        int field2 = 10;
        [ValidateInt32(2, 6, true)]
        int field3 = 20;
        [ValidateInt32(0, 50, false)]
        public int MyProperty { get; set; }
        [ValidateInt32(-3, 100, true)]
        public int MyProperty2 { get; set; }
        [ValidateInt32(0, 5, true)]
        public string StringProp { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            MyType type = new MyType();
            Int32Validate.Validate(type);
        }
    }
}
