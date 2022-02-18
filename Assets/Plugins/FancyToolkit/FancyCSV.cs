using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Nortal.Utilities.Csv;
using System.Globalization;

public static class FancyCSV
{
    public static List<T> FromText<T>(string text) where T: new()
    {
        return new FancySheet(text).Convert<T>();
    }

    public static List<T> FromFile<T>(string filePath) where T : new()
    {
        FancySheet result = null;
        using (StreamReader sr = new StreamReader(filePath))
        {
            result = new FancySheet(sr);
        }

        return result?.Convert<T>();
    }

    public class FancySheet
    {
        string[] keys;
        string[][] values;

        CsvSettings Settings => new CsvSettings() { FieldDelimiter = '|' };

        public FancySheet(StreamReader streamReader)
        {
            using (var parser = new CsvParser(streamReader, Settings))
            {
                var vals = new List<string[]>(); 
                while(parser.HasMoreRows)
                {
                    var row = parser.ReadNextRow();

                    if (row == null) break;
                    
                    vals.Add(row);
                }

                values = vals.ToArray();
            }

            keys = values[0];
        }


        public FancySheet(string content)
        {
            values = CsvParser.Parse(content, Settings);

            keys = values[0];
        }

        public static System.Reflection.MethodInfo GetStingConvertMethod(Type t)
        {
            return t
                .GetMethods()
                .FirstOrDefault(m => m.GetCustomAttributes(typeof(CreateFromStringAttribute), false).Length > 0);
        }

        static object ConvertFromString(Type t, object s)
        {
            return GetStingConvertMethod(t).Invoke(null, new[] { s });
        }

        static bool IsStringConvertible(Type t)
        {
            return GetStingConvertMethod(t) != null;
        }

        static bool IsGenericList(Type t, out Type elementType)
        {
            if (t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(List<>)))
            {
                elementType = t.GetGenericArguments()[0];
                return true;
            }

            elementType = null;
            return false;
        }

        T Convert<T>(string[] row) where T : new()
        {
            var item = new T();
            var type = typeof(T);

            for (int i = 0; i < keys.Length; i++)
            {
                string value = i < row.Length ? row[i] : "";

                var field = type.GetField(keys[i]);
                if (field == null) continue;
                var fType = field.FieldType;

                var obj = ConvertOne(fType, value);
                if (obj != null)
                {
                    field.SetValue(item, obj);
                }
                else
                {
                    Console.WriteLine($"Warning: Unconvertable type {fType}");
                }

            }

            return item;
        }

        public object ConvertOne(Type fType, string value)
        {
            if (value == null) value = "";

            if (fType.IsEnum)
            {
                return Enum.Parse(fType, value, true);
            }
            else if (fType == typeof(float))
            {
                return GetFloat(value);
            }
            else if (fType == typeof(int))
            {
                return GetInt(value);
            }
            else if (fType == typeof(string))
            {
                return GetString(value);
            }
            else if (IsStringConvertible(fType))
            {
                return ConvertFromString(fType, value);
            }
            else if (IsGenericList(fType, out var eType))
            {
                var list = Activator.CreateInstance(fType);

                var addMethod = fType.GetMethod("Add");

                foreach (var s in GetString(value).Split(';').Select(x => x.Trim()))
                {
                    var o = ConvertOne(eType, s);
                    if (o == null) return null; //means we can't convert

                    addMethod.Invoke(list, new[] { o });
                }

                return list;
            }

            return null;
        }


        public List<T> Convert<T>() where T : new()
        {
            var result = new List<T>();

            for (int i = 1; i < values.Length; i++)
            {
                var item = values[i];

                // skip empty rows
                if (item.All(string.IsNullOrWhiteSpace)) continue;
                result.Add(Convert<T>(item));
            }

            return result;
        }

        public static TEnum GetEnum<TEnum>(string obj) where TEnum : struct
        {
            if (Enum.TryParse(obj, true, out TEnum value))
            {
                return value;
            }

            return default;
        }

        public static float GetFloat(string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {
                return float.Parse(obj, CultureInfo.InvariantCulture);
            }

            return 0f;
        }

        public static int GetInt(string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {
                return int.Parse(obj);
            }

            return 0;
        }

        public static string GetString(string obj) => obj;
    }
}

public class CreateFromStringAttribute : Attribute
{

}

public static class ReflectionHelpersForSheets
{
    public static object InvokeStatic(this System.Reflection.MethodInfo method, params object[] args) => method.Invoke(null, args);
}