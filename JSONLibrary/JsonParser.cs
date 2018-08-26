using System;
using System.ComponentModel;
using System.Diagnostics;
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
            this.json = new StringReader(string.Concat(json.Where(c => !char.IsWhiteSpace(c))));
        }

        public static object Parse(string json) {
            using (var instance = new JsonParser(json)) {
                return instance.InitParse();
            }
        }

        private object InitParse() {
            return ParseValue(getNextToken());
        }
        
        private TOKEN getNextToken() {
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
                    return TOKEN.Comma;
                case '"':
                    return TOKEN.String;
                case ':':
                    return TOKEN.Colon;
                // only integer numbers ?
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
            while (serviceCharacters.IndexOf(Convert.ToChar(json.Peek())) != -1 && json.Peek() != -1) {
                builder.Append(Convert.ToChar(json.Read()));
            }

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

        private object ParseObject() {
            // TODO
            return null;
        }

        private object ParseArray() {
            // TODO
            return null;
        }

        private string ParseString() {
            json.Read();
            StringBuilder builder = new StringBuilder();

            bool stringIsGoing = true;
            while (json.Peek() != -1) {
                
                char nextChar = Convert.ToChar(json.Read());
                switch (nextChar) {
                    case '"': // '\"'
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