//need to define if using JSON_DOT_NET
#define JSON_DOT_NET

#if (JSON_DOT_NET)
using Newtonsoft.Json;
#endif

namespace IEC
{
    public class JsonDotNetModule : IJsonService
    {
#if (JSON_DOT_NET)
        public string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        // public JObject ToJObject(string json)
        // {
        //     return JObject.Parse(json);
        // }
#else
        public T FromJson<T>(string json)
        {
            throw new NotImplementedException();
        }

        public string ToJson(object obj)
        {
            throw new NotImplementedException();
        }
#endif
    }
}

