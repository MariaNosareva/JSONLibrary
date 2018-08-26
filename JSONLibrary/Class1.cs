using System;
using System.Collections;
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
            return new T();
        }
    }
}