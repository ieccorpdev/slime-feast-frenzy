using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using IEC;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class ReflectionExtensions
{
    public static bool ImplementsIData(this FieldInfo fieldInfo)
    {
        if (fieldInfo == null) throw new ArgumentNullException(nameof(fieldInfo));

        var fieldType = fieldInfo.FieldType;
        return typeof(IData).IsAssignableFrom(fieldType);
    }

    public static void CopyValuesFrom(this object target, object source, HashSet<object> visited = null)
    {
        if (source == null || target == null)
        {
            throw new ArgumentNullException("Source or Destination cannot be null");
        }

        if (visited == null)
            visited = new HashSet<object>();

        // Prevent infinite recursion in case of circular references
        if (visited.Contains(source))
            return;

        visited.Add(source);

        Type sourceType = source.GetType();
        Type destinationType = target.GetType();

        // Handle JObject (from JSON)
        if (source is JObject jObjectSource)
        {
            foreach (var property in jObjectSource.Properties())
            {
                CopyPropertyOrField(property.Name, property.Value, target);
            }

            return;
        }

        // Handle Dictionary (JSON-Deserialized Object)
        if (source is IDictionary<string, object> dictionarySource)
        {
            foreach (var kvp in dictionarySource)
            {
                CopyPropertyOrField(kvp.Key, kvp.Value, target);
            }

            return;
        }

        // Copy fields (matching names)
        foreach (FieldInfo sourceField in sourceType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            FieldInfo destinationField = destinationType.GetField(sourceField.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (destinationField != null)
            {
                object sourceValue = sourceField.GetValue(source);
                object convertedValue = ConvertValue(sourceValue, destinationField.FieldType);
                destinationField.SetValue(target, convertedValue);
            }
        }

        // Copy properties (matching names)
        foreach (PropertyInfo sourceProp in sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            PropertyInfo destinationProp = destinationType.GetProperty(sourceProp.Name, BindingFlags.Public | BindingFlags.Instance);
            if (destinationProp != null && destinationProp.CanWrite && sourceProp.CanRead)
            {
                object sourceValue = sourceProp.GetValue(source);
                object convertedValue = ConvertValue(sourceValue, destinationProp.PropertyType);
                destinationProp.SetValue(target, convertedValue);
            }
        }
    }

    private static void CopyPropertyOrField(string name, object sourceValue, object destination)
    {
        Type destinationType = destination.GetType();
        FieldInfo destinationField = destinationType.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        PropertyInfo destinationProperty = destinationType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);

        if (destinationField != null)
        {
            object convertedValue = ConvertValue(sourceValue, destinationField.FieldType);
            destinationField.SetValue(destination, convertedValue);
        }
        else if (destinationProperty != null && destinationProperty.CanWrite)
        {
            object convertedValue = ConvertValue(sourceValue, destinationProperty.PropertyType);
            destinationProperty.SetValue(destination, convertedValue);
        }
    }

    private static object ConvertValue(object value, Type targetType)
    {
        if (value == null) return null;

        // Convert JObject to the required type
        if (value is JObject jObject)
        {
            return jObject.ToObject(targetType);
        }

        // Convert Dictionary (from JSON) into the target type
        if (value is IDictionary<string, object> dictionary)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(Newtonsoft.Json.JsonConvert.SerializeObject(dictionary), targetType);
        }

        // Convert JArray (for Lists/Arrays)
        if (value is JArray jArray)
        {
            return jArray.ToObject(targetType);
        }

        // Convert primitive types
        if (targetType.IsAssignableFrom(value.GetType()))
        {
            return value;
        }

        // Handle collections manually
        if (typeof(IEnumerable).IsAssignableFrom(targetType) && targetType != typeof(string))
        {
            return ConvertList(value, targetType);
        }

        // Convert other object types if possible
        try
        {
            return Convert.ChangeType(value, targetType);
        }
        catch
        {
            return null;
        }
    }

    private static object ConvertList(object value, Type targetType)
    {
        if (!(value is IEnumerable sourceList))
            return null;

        Debug.LogError($"ConvertList: {value}, TargetType: {targetType}");
        Type elementType = targetType.IsArray ? targetType.GetElementType() : targetType.GenericTypeArguments[0];
        IList newList = (IList)Activator.CreateInstance(targetType);

        foreach (var item in sourceList)
        {
            newList.Add(ConvertValue(item, elementType));
        }

        return newList;
    }

    private static bool IsPrimitiveOrString(Type type)
    {
        return type.IsPrimitive || type == typeof(string) || type == typeof(decimal);
    }
}