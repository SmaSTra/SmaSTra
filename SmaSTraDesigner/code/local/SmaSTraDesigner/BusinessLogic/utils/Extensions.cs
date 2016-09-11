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

        public static JToken GetValue(this JObject obj, string key, JToken defaultValue)
        {
            JToken token = obj.GetValue(key);
            return token == null ? defaultValue : token;
        }


        public static JObject GetValueAsJObject(this JObject obj, string key, JObject defaultValue)
        {
            JObject token = obj.GetValue(key) as JObject;
            return token == null ? defaultValue : token;
        }


        public static string GetValueAsString(this JObject obj, string key, string defaultValue)
        {
            JToken token = obj.GetValue(key);
            return token == null ? defaultValue : token.ToString();
        }
    }


    static class DictionaryExtensions
    {


        public static TValue TryGetValue<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dic, TKey key, TValue defaultValue)
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

    }

    static class IEnumerations
    {

        /// <summary>
        /// Extension Method for a simple ForEach.
        /// </summary>
        /// <typeparam name="T">To use in the Function / Collection</typeparam>
        /// <param name="elements">To Iterate</param>
        /// <param name="func">To execute</param>
        public static void forEach<T>(this IEnumerable<T> elements, Action<T> func)
        {
            if (elements == null || func == null) return;
            foreach( T element in elements )
            {
                func.Invoke(element);
            }
        }

    }

}
