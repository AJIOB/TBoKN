using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;

namespace lab4.Enums.Utilities
{
    public static class EnumViewUtility
    {
        public static object[] GetValuesAndDescriptions(Type enumType)
        {
            var values = Enum.GetValues(enumType).Cast<object>();
            var valuesAndDescriptions = from value in values
                select new
                {
                    Value = value,
                    Description = value.GetType()
                        .GetMember(value.ToString())[0]
                        .GetCustomAttributes(true)
                        .OfType<DescriptionAttribute>()
                        .First()
                        .Description
                };
            return valuesAndDescriptions.ToArray();
        }
        
        public static object[] GetValuesAndDescriptionsToString(Type enumType)
        {
            var values = Enum.GetValues(enumType).Cast<object>();
            var valuesAndDescriptions = from value in values
                select new
                {
                    Value = value,
                    Description = value.ToString()
                };
            return valuesAndDescriptions.ToArray();
        }

        public static object[] GetValuesAndDescriptionsStopBits()
        {
            var valuesDictionary = new Dictionary<StopBits, string> {
                {StopBits.One, "1"},
                {StopBits.OnePointFive, "1.5"},
                {StopBits.Two, "2"}
            };
            var valuesAndDescriptions = from value in valuesDictionary
                select new
                {
                    Value = value.Key,
                    Description = value.Value
                };
            return valuesAndDescriptions.ToArray();
        }
        
        public static object[] GetValuesAndDescriptionsFlowControl()
        {
            var valuesDictionary = new Dictionary<object, string> {
                {StopBits.None, "None"}
            };
            var valuesAndDescriptions = from value in valuesDictionary
                select new
                {
                    Value = value.Key,
                    Description = value.Value
                };
            return valuesAndDescriptions.ToArray();
        }
    }
}