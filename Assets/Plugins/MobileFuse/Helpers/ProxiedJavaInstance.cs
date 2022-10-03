using UnityEngine;

namespace Plugins.MobileFuse.Helpers
{
    public abstract class ProxiedJavaInstance
    {
        protected readonly AndroidJavaObject JavaInstance;
        
        public ProxiedJavaInstance(string className, params object[] args) => JavaInstance = new AndroidJavaObject(className, args);
        
        // Proxied internal java calls:
        protected T Call<T>(string methodName, params object[] args) => JavaInstance.Call<T>(methodName, args);
        protected void Call(string methodName, params object[] args) => JavaInstance.Call(methodName, args);
        protected T CallStatic<T>(string methodName, params object[] args) => JavaInstance.CallStatic<T>(methodName, args);
        protected void CallStatic(string methodName, params object[] args) => JavaInstance.CallStatic(methodName, args);
        protected T Get<T>(string fieldName) => JavaInstance.Get<T>(fieldName);
        protected void Set<T>(string fieldName, T val) => JavaInstance.Set(fieldName, val);
        protected T GetStatic<T>(string fieldName) => JavaInstance.GetStatic<T>(fieldName);
        protected void SetStatic<T>(string fieldName, T val) => JavaInstance.SetStatic(fieldName, val);
    }
}