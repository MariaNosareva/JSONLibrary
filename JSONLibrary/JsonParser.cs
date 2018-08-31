using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JSONLibrary {

    public class JsonParser : IDisposable {

        private readonly string serviceCharacters = "[]{}:,\"";

        enum TOKEN {
            OpenBrace,
            CloseBrace,
            OpenBracket,
            CloseBracket,
            Colon,
            Comma,
            Null,
            String,
            Number,
            False,
            True,
            None
        }

        private StringReader json;

        public JsonParser(string json) {
            this.json = new StringReader(json);//;
        }

        public static object Parse(string json) {
            using (var instance = new JsonParser(json)) {
                return instance.InitParse();
            }
        }

        private object InitParse() {
            return ParseValue(getNextToken());
        }
        
        void EatWhitespace() {
            while (Char.IsWhiteSpace(Convert.ToChar(json.Peek()))) {
                json.Read();

                if (json.Peek() == -1) {
                    break;
                }
            }
        }
        
        private TOKEN getNextToken() {
            
            EatWhitespace();
            
            if (json.Peek() == -1) {
                return TOKEN.None;
            }
            
            switch (Convert.ToChar(json.Peek())) {
                case '{':
                    return TOKEN.OpenBrace;
                case '}':
                    json.Read();
                    return TOKEN.CloseBrace;
                case '[':
                    return TOKEN.OpenBracket;
                case ']':
                    json.Read();
                    return TOKEN.CloseBracket;
                case ',':
                    json.Read();
                    Console.WriteLine("hello comma");
                    return TOKEN.Comma;
                case '\"':
                    return TOKEN.String;
                case ':':
                    return TOKEN.Colon;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                    return TOKEN.Number;
            }

            switch (getNextWord()) {
                case "true":
                    Console.WriteLine("token true");
                    return TOKEN.True;
                case "false":
                    return TOKEN.False;
                case "null":
                    return TOKEN.Null;
            }
            
            return TOKEN.None;
        }
        
        private string getNextWord() {
            StringBuilder builder = new StringBuilder();
            while (serviceCharacters.IndexOf(Convert.ToChar(json.Peek())) == -1 && json.Peek() != -1) {
                builder.Append(Convert.ToChar(json.Read()));
            }
            Console.WriteLine(builder.ToString());
            return builder.ToString();
        }
        
        private object ParseValue(TOKEN token) {
            switch (token) {
                case TOKEN.OpenBrace:
                    return ParseObject();
                case TOKEN.OpenBracket:
                    return ParseArray();
                case TOKEN.String:
                    return ParseString();
                case TOKEN.Number:
                    return ParseNumber();
                case TOKEN.True:
                    return true;
                case TOKEN.False:
                    return false;
                case TOKEN.Null:
                    return null;
                default:
                    return null;
            }
        }

        private Dictionary<string, object> ParseObject() {
            json.Read();
            Dictionary<string, object> map = new Dictionary<string, object>();

            while (json.Peek() != -1) {
                switch (getNextToken()) {
                    case TOKEN.CloseBrace:
                        return map;
                    case TOKEN.Comma:
                        continue;
                    default:
                        string key = ParseString();
                        if (key == null || getNextToken() != TOKEN.Colon) {
                            throw new ArgumentException("incorrect object");
                        }

                        json.Read();
                        map.Add(key, InitParse());
                        break;
                }
            }
            throw new ArgumentException("incorrect object");
        }

        private ArrayList ParseArray() {
            json.Read();
            ArrayList array = new ArrayList();

            while (json.Peek() != -1) {
                TOKEN token = getNextToken();
                switch (token) {
                    case TOKEN.Comma:
                        continue;
                    case TOKEN.CloseBracket:
                        foreach (var elem in array) {
                            Console.WriteLine("ARRAY " + elem);
                        }
                        return array;
                    default:
                        object element = ParseValue(token);
                        array.Add(element);
                        break;
                }
            }

            throw new ArgumentException("incorrect array");
        }

        private string ParseString() {
            json.Read();
            StringBuilder builder = new StringBuilder();

            while (json.Peek() != -1) {
                char nextChar = Convert.ToChar(json.Read());

                switch (nextChar) {
                    case '\"': // '\"'
                        return builder.ToString(); // end of string
                    case '\\':
                        
                        char additional = Convert.ToChar(json.Read());
                        switch (additional) {
                            case 'n':
                                builder.Append("\n");
                                break;
                            case 'b':
                                builder.Append("\b");
                                break;
                            case 'f':
                                builder.Append("\f");
                                break;
                            case 'r':
                                builder.Append("\r");
                                break;
                            case 't':
                                builder.Append("\t");
                                break;
                            case '\\':
                                builder.Append("\\");
                                break;
                            case '/':
                                builder.Append("/");
                                break;
                            case 'u':
                                // TODO hex
                                break;
                        }

                        break;
                    default:
                        builder.Append(nextChar);
                        break;
                }
            }
            throw new ArgumentException("incorrect string while parsing");
        }

        private object ParseNumber() {
            string supposedNumber = getNextWord();
            if (supposedNumber.Contains('.')) {
                return Double.Parse(supposedNumber);
            }
            return Int64.Parse(supposedNumber);
        }

        public void Dispose() {
            json.Dispose();
        }
    }
}