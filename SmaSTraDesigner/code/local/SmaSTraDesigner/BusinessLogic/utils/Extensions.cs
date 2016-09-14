using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaSTraDesigner.BusinessLogic.utils
{
    static class JsonExtensions
    {

        public static JToken GetValue(this JObject obj, string key, JToken defaultValue = null)
        {
            JToken token = obj.GetValue(key);
            return token == null ? defaultValue : token;
        }


        public static JObject GetValueAsJObject(this JObject obj, string key, JObject defaultValue = null)
        {
            JObject token = obj.GetValue(key) as JObject;
            return token == null ? defaultValue : token;
        }


        public static JArray GetValueAsJArray(this JObject obj, string key, JArray defaultValue = null)
        {
            JArray token = obj.GetValue(key) as JArray;
            return token == null ? defaultValue : token;
        }


        public static string GetValueAsString(this JObject obj, string key, string defaultValue = "")
        {
            JToken token = obj.GetValue(key);
            return token == null ? defaultValue : token.ToString();
        }


        public static int GetValueAsInt(this JObject obj, string key, int defaultValue = 0)
        {
            JToken token = obj.GetValue(key);
            if (token != null)
            {
                Int32.TryParse(token.ToString(), out defaultValue);
            }

            return defaultValue;
        }


        public static string[] GetValueAsStringArray(this JObject obj, string key, string[] defaultValue = null )
        {
            JArray array = obj.GetValue(key) as JArray;
            return array == null ? defaultValue : array.CollectToStringArray();
        }


        /// <summary>
        /// Collects an Json Array to a string[].
        /// </summary>
        /// <param name="array">To collect</param>
        /// <returns>the String array. Returns an empty array if null.</returns>
        public static string[] CollectToStringArray(this JArray array)
        {
            if (array == null) return new string[0];
            return array
                .Select(t => t.ToString())
                .ToArray();
        }

        public static T[] Collect<T>(this JArray array, Func<JToken, T> func)
        {
            return array
                .Select(t => func.Invoke(t))
                .ToArray();
        }
        

        public static IEnumerable<JObject> ToJObj(this JArray array)
        {
            return array
                .Select(t => t as JObject);
        }


        /// <summary>
        /// Creates a String -> String dict from an String -> Object dict.
        /// </summary>
        /// <param name="dic">To convert</param>
        /// <returns>The converted Dict.</returns>
        public static IDictionary<string, string> ToStringString(this IDictionary<string, JToken> dic)
        {
            Dictionary<string, string> newDict = new Dictionary<string, string>();
            dic.ForEach(e => newDict.Add(e.Key, e.Value.ToString()));

            return newDict;
        }


        public static IEnumerable<T> NonNull<T>(this IEnumerable<T> collection)
        {
            return collection.Where(e => e != null);
        }

    }


    static class StringExtensions
    {

        public static string EmptyDefault(this string val, string defaultValue)
        {
            return String.IsNullOrEmpty(val) ? defaultValue : val;
        }

    }


    static class DictionaryExtensions
    {


        public static TValue GetValue<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dic, TKey key, TValue defaultValue)
        {
            try
            {
                var val = dic.First(e => object.Equals(e.Key,key));
                return val.Value;
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception e)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                return defaultValue;
            }
        }


        public static TKey GetKeyForValue<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dic, TValue value, TKey defaultValue)
        {
            try
            {
                var val = dic.First(e => object.Equals(e.Value, value));
                return val.Key;
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception e)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                return defaultValue;
            }
        }


        public static double TryGetValueAsDouble(this IEnumerable<KeyValuePair<string, string>> dic, string key, out double outVal, double defaultValue)
        {
            try
            {
                var val = dic.First(e => object.Equals(e.Key, key)).Value;
                if (val != null) Double.TryParse(val, out defaultValue);

                outVal = defaultValue;
                return defaultValue;
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception e)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                outVal = defaultValue;
                return defaultValue;
            }
        }

    }

    static class IEnumerations
    {

        /// <summary>
        /// Extension Method for a simple ForEach.
        /// </summary>
        /// <typeparam name="T">To use in the Function / Collection</typeparam>
        /// <param name="elements">To Iterate</param>
        /// <param name="func">To execute</param>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> elements, Action<T> func)
        {
            if (func == null) return elements;
            foreach( T element in elements )
            {
                func.Invoke(element);
            }

            return elements;
        }


        /// <summary>
        /// Simple check if collection is empty.
        /// </summary>
        /// <typeparam name="T">To use in the Function / Collection</typeparam>
        /// <param name="elements">To Iterate</param>
        public static bool Empty<T>(this IEnumerable<T> elements)
        {
            return !elements.Any();
        }

    }

}
