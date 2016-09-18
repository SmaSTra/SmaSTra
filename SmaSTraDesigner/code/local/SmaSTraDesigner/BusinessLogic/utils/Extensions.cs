using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public static double GetValueAsDouble(this JObject obj, string key, double defaultValue = 0)
        {
            JToken token = obj.GetValue(key);
            if (token != null)
            {
                Double.TryParse(token.ToString(), out defaultValue);
            }

            return defaultValue;
        }

        public static bool GetValueAsBool(this JObject obj, string key, bool defaultValue = true)
        {
            JToken token = obj.GetValue(key);
            if (token != null)
            {
                bool.TryParse(token.ToString(), out defaultValue);
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

        public static JArray ToJArray(this IEnumerable<Object> array)
        {
            JArray jArray = new JArray();
            array.ForEach(jArray.Add);
            return jArray;
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

        public static JObject ToJObject(this IDictionary<string,string> dic)
        {
            JObject obj = new JObject();
            dic.ForEach(e => obj.Add(e.Key, e.Value));
            return obj;
        }


        public static IEnumerable<T> NonNull<T>(this IEnumerable<T> collection)
        {
            return collection.Where(e => e != null);
        }

        public static string ToBeautifulJson(this JToken token)
        {
            var jsonSettings = new JsonSerializerSettings()
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Newtonsoft.Json.Formatting.Indented
            };

            //Generate a string and write it to the file:
            return JsonConvert.SerializeObject(token, jsonSettings);
        }

    }


    static class StringExtensions
    {

        public static string EmptyDefault(this string val, string defaultValue)
        {
            return String.IsNullOrEmpty(val) ? defaultValue : val;
        }

        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static string StringJoin(this IEnumerable<string> split, string mergeString)
        {
            return string.Join(mergeString, split);
        }

        public static string StringJoin(this IEnumerable<string> split)
        {
            return string.Join("", split);
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
        /// Extension Method for a simple ForEach.
        /// </summary>
        /// <typeparam name="T">To use in the Function / Collection</typeparam>
        /// <param name="elements">To Iterate</param>
        /// <param name="func">To execute</param>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> elements, Action<T,int> func)
        {
            if (func == null) return elements;
            
            int i = 0;
            return elements.ForEach(e => { func.Invoke(e, i); i++; });
        }



        /// <summary>
        /// Extension Method for a simple ForEach.
        /// </summary>
        /// <typeparam name="T">To use in the Function / Collection</typeparam>
        /// <param name="elements">To Iterate</param>
        /// <param name="func">To execute</param>
        public static IEnumerable<T> ForEachNonNull<T>(this IEnumerable<T> elements, Action<T, int> func)
        {
            if (func == null) return elements;
            int i = 0;
            foreach (T element in elements)
            {
                if(element != null) func.Invoke(element, i);
                i++;
            }

            return elements;
        }


        public static bool ExecuteOnFirst<T>(this IEnumerable<T> elements, Action<T> action)
        {
            if (elements.Empty()) return false;
            else action.Invoke(elements.First());
            return true;
        }

        public static bool ExecuteOnFirst<T>(this IEnumerable<T> elements, Func<T, bool> search, Action<T> action)
        {
            return elements.Where(search).ToList().ExecuteOnFirst(action);
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
