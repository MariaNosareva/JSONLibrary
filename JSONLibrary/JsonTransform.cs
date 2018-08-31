using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;

namespace JSONLibrary {
    public static class ObjectTransform {

        public static StringBuilder valueToString(this Object value, StringBuilder valueResult) {
            switch (value.GetType().Name) {
                case "String":
                    valueResult.Append("\"" + value + "\"");
                    break;
                case "ArrayList":
                    valueResult.Append("[");
                    bool notEmpty = false;
                    foreach (var element in (ArrayList) value) {
                        valueResult = element.valueToString(valueResult);
                        valueResult.Append(", ");
                        notEmpty = true;
                    }

                    if (notEmpty) {
                        valueResult.Remove(valueResult.Length - 2, 2);
                    }

                    valueResult.Append("]");
                    break;
                case "Int32":
                case "Int64":
                case "Single":
                case "Double":
                    valueResult.Append(value);
                    break;
                case "Boolean":
                    valueResult.Append(value.ToString().ToLower());
                    break;
                default:
                    valueResult.Append(value.ToJson());
                    break;
            }

            return valueResult;
        }

        
        public static String ToJson(this Object obj) {

            var jsonBuilder = new StringBuilder("{");
            var type = obj.GetType();

            bool notEmpty = false;
            foreach (var property in type.GetProperties()) {
                jsonBuilder.Append("\"" + property.Name + "\":");

                var value = property.GetValue(obj, new object[] { });
                var valueResult = new StringBuilder();
                if (value == null) {
                    valueResult.Append("null");
                }
                else {
                    valueResult = value.valueToString(valueResult);
                }
                
                jsonBuilder.Append(valueResult);
                jsonBuilder.Append(", ");
                notEmpty = true;
            }

            if (notEmpty) {
                jsonBuilder.Remove(jsonBuilder.Length - 2, 2);
            }
            
            var str = jsonBuilder.Append("}").ToString();
            return str;
        }


        public static void ToJsonFile(this Object obj, string fileName) {
            String result = obj.ToJson();
            using (var textWriter = new StreamWriter(File.Open(fileName, FileMode.Append))) {
                textWriter.WriteLine(result);
                textWriter.Flush();
            }
        }
    }

    public static class StringTransform {


        public static T FromJson<T>(this String str) where T: new() {
            var map = (Dictionary<string, object>)JsonParser.Parse(str);
//            foreach (var key in map.Keys) {
//                Console.WriteLine(key);
//            }            
            return (T)(new JsonToObject()).DeserialiseMap(map, typeof(T), new T());
        }

        
    }

    internal class JsonToObject {
        
        public object DeserialiseMap(Dictionary<string, object> map, Type type, object destination) {
            
            if (type == typeof(Object) || type == typeof(Dictionary<string, object>)) {
                return map;
            }

            foreach (var propertyInfo in type.GetProperties()) {
                String name = propertyInfo.Name;
                if (!map.ContainsKey(name)) {
                    throw new InvalidExpressionException("no such field in object");
                }
                switch (propertyInfo.PropertyType.Name) {
                    case "Single":
                    case "Boolean":
                    case "ArrayList":
                    case "Double":
                    case "String":
                    case "Int32":
                        propertyInfo.SetValue(destination, map[name], null);
                        break;
                    default:
                        
                        if (map[name].GetType() == typeof(Dictionary<string, object>)) {
                           // propertyInfo.SetValue(destination, DeserialiseMap((Dictionary<string, object>)map[name], propertyInfo.GetType(), destination.));
                        }
                        break;
                }
            }
            
            return destination;
        }
    }
}