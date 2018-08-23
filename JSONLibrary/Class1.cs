using System;
using System.Collections;
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
                    foreach (var element in (ArrayList) value) {
                        valueResult = element.valueToString(valueResult);
                        valueResult.Append(", ");
                    }

                    valueResult.Remove(valueResult.Length - 2, 2).Append("]");
                    break;
                case "Int32":
                    valueResult.Append(value);
                    break;
                case "Single":
                    valueResult.Append(value);
                    break;
                case "Double":
                    valueResult.Append(value);
                    break;
                case "Boolean":
                    valueResult.Append(value.ToString().ToLower());
                    break;
            }

            return valueResult;
        }

        public static String ToJson(this Object obj) {

            var jsonBuilder = new StringBuilder("{");
            var type = obj.GetType();
            
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
            }
            
            var str = jsonBuilder.Remove(jsonBuilder.Length-2, 2).Append("}").ToString();
            return str;
        }

        public static void ToJsonFile(this Object obj, string fileName) { }
    }

    public static class StringTransform {
        public static T FromJson<T>(this String str) where T: new() {
            return new T();
        }
    }
}