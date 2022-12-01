using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
        public string FieldName { get; set; }

        public string[] ListOfRestrictions { get; set; }

        public int CurrentValue { get; set; }

        public string MyMessage { get; } = "Field or Property does not meet criteria!";

        public InvalidValueException(string fieldName, int currentValue, ValidateInt32Attribute validateInt32Attribute)
        {
            FieldName = fieldName;
            CurrentValue = currentValue;
            ListOfRestrictions = new string[3];
            ListOfRestrictions[0] = validateInt32Attribute.MinValue.ToString();
            ListOfRestrictions[1] = validateInt32Attribute.MaxValue.ToString();
            ListOfRestrictions[2] = validateInt32Attribute.ZeroEnabled.ToString();
        }
    }

    static class Int32Validate
    {
        public static void Validate(object obj)
        {
            PropertyInfo[] propList = obj.GetType().GetProperties();
            FieldInfo[] fieldList = obj.GetType().GetFields();
            MemberInfo[] list = new MemberInfo[propList.Length + fieldList.Length];
            
            for (int i = 0; i < fieldList.Length; i++)
                list[i] = fieldList[i];
            int t = propList.Length;
            for (int i = 0; i < propList.Length; i++)
                list[i + t] = propList[i];

            Int32 check = 0;
            foreach (var item in list)
            {
                try
                {
                    if (item.IsDefined(typeof(ValidateInt32Attribute)))
                    {
                        ValidateInt32Attribute currentAttribute = (ValidateInt32Attribute)item.GetCustomAttribute(typeof(ValidateInt32Attribute));
                        switch (item.MemberType)
                        {
                            case MemberTypes.Field:
                                if ((int)((FieldInfo)item).GetValue(obj) < currentAttribute.MinValue ||
                                (int)((FieldInfo)item).GetValue(obj) > currentAttribute.MaxValue ||
                                !currentAttribute.ZeroEnabled ||
                                item.GetType() == check.GetType())
                                {
                                    throw new InvalidValueException(item.Name, (int)((FieldInfo)item).GetValue(obj), currentAttribute);
                                }
                                break;
                            case MemberTypes.Property:
                                if ((int)((PropertyInfo)item).GetValue(obj) < currentAttribute.MinValue ||
                                (int)((PropertyInfo)item).GetValue(obj) > currentAttribute.MaxValue ||
                                !currentAttribute.ZeroEnabled ||
                                item.GetType() == check.GetType())
                                {
                                    throw new InvalidValueException(item.Name, (int)((PropertyInfo)item).GetValue(obj), currentAttribute);
                                }
                                break;
                        }


                    }
                }
                catch (InvalidValueException e)
                {

                    Console.WriteLine($"InvalidValueException \n" +
                        $"Field name: {e.FieldName}\n" +
                        $"List of Restrictions: {e.ListOfRestrictions[0]}, " +
                        $"{e.ListOfRestrictions[1]}, " +
                        $"{e.ListOfRestrictions[2]} \n" +
                        $"Current value: {e.CurrentValue}\n" +
                        $"Message: {e.MyMessage}\n");
                }

            }

        }
    }

    class MyType
    {
        [ValidateInt32(1, 5, true)]
        public int field1 = 5;
        [ValidateInt32(5, 32, false)]
        public int field2 = 10;
        [ValidateInt32(2, 6, true)]
        public int field3 = 20;
        [ValidateInt32(0, 50, false)]
        public int MyProperty { get; set; }
        [ValidateInt32(-3, 100, true)]
        public int MyProperty2 { get; set; }

        public string StringProp { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            MyType type = new MyType();
            type.MyProperty = 0;
            type.MyProperty2 = 2;
            type.StringProp = "hello";
            Int32Validate.Validate(type);
        }
    }
}
