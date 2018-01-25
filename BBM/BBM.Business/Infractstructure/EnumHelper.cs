using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace BBM.Business.Infractstructure
{
    public static class EnumHelper<T>
    {
        public class displayEnum
        {
            public string displayLabel { get; set; }
            public string displayCode { get; set; }
        }

        public static Dictionary<int, displayEnum> ToArray(Type enumType)
        {
            Dictionary<int, displayEnum> listEnumField = new Dictionary<int, displayEnum>();
            Type type = enumType;
            foreach (var evalue in type.GetEnumValues())
            {
                var val = (int)evalue;
                var valueName = type.GetField(evalue.ToString());

                DisplayAttribute displayAtt = valueName.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
                var displayEnumValue = new displayEnum();
                if (displayAtt != null)
                {
                    displayEnumValue.displayLabel = displayAtt.GetName();
                    displayEnumValue.displayCode = displayAtt.GetShortName();
                }

                listEnumField.Add((int)evalue, displayEnumValue);
            }
            return listEnumField;
        }

        public static string ToJson(Type enumType)
        {
            var listEnumField = ToArray(enumType);
            return JsonConvert.SerializeObject(listEnumField.Select(m => new { Key = m.Key, Value = m.Value.displayLabel, Code = m.Value.displayCode }));
        }

        public static Dictionary<int, string> GetListValueEnum(Type enumType)
        {
            Dictionary<int, string> listEnumField = new Dictionary<int, string>();
            Type type = enumType;
            foreach (var evalue in type.GetEnumValues())
            {
                listEnumField.Add((int)evalue, evalue.ToString());
            }
            return listEnumField;
        }
        public static string ToKeyValueJson(Type enumType)
        {
            Dictionary<int, string> listEnumField = new Dictionary<int, string>();
            Type type = enumType;
            foreach (var evalue in type.GetEnumValues())
            {
                listEnumField.Add((int)evalue, evalue.ToString());
            }
            return JsonConvert.SerializeObject(listEnumField.Select(m => new { Key = m.Key, Value = m.Value }));
        }

        public static IList<T> GetValues(Enum value)
        {
            var enumValues = new List<T>();

            foreach (FieldInfo fi in value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumValues.Add((T)Enum.Parse(value.GetType(), fi.Name, false));
            }
            return enumValues;
        }

        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }


        public static IList<string> GetNames(Enum value)
        {
            return value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
        }

        public static IList<string> GetDisplayValues(Enum value)
        {
            return GetNames(value).Select(obj => GetDisplayValue(Parse(obj))).ToList();
        }

        private static string lookupResource(Type resourceManagerProvider, string resourceKey)
        {
            foreach (PropertyInfo staticProperty in resourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (staticProperty.PropertyType == typeof(System.Resources.ResourceManager))
                {
                    System.Resources.ResourceManager resourceManager = (System.Resources.ResourceManager)staticProperty.GetValue(null, null);
                    return resourceManager.GetString(resourceKey);
                }
            }

            return resourceKey; // Fallback with the key name
        }

        public static string GetDisplayValuebyInt(int value)
        {
            Type enumType = typeof(T);

            if (!enumType.IsEnum)
            {
                throw new Exception("T must be an Enumeration type.");
            }

            var valueEnum = (T)Enum.ToObject(enumType, value);

            return GetDisplayValue(valueEnum);
        }

        public static string GetDisplayValue(T value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes[0].ResourceType != null)
                return lookupResource(descriptionAttributes[0].ResourceType, descriptionAttributes[0].Name);

            if (descriptionAttributes == null) return string.Empty;
            return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
        }
    }
}