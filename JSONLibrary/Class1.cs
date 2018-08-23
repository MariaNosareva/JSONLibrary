using System;

namespace JSONLibrary {
    public static class ObjectTransform {
        public static String ToJson(this Object obj) {
            return "";
        }

        public static void ToJsonFile(this Object obj, string fileName) { }
    }

    public static class StringTransform {
        public static T FromJson<T>(this String str) where T: new() {
            return new T();
        }
    }
}