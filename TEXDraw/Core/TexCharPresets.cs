using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace TexDrawLib
{
    public enum ImportCharPresetsType
    {
        Legacy = 0,
        ASCII = 1,
        FullUnicode = 2,
        Alphanumeric = 3,
        Custom = -1,
    }
    public class TexCharPresets
    {

        public const string legacyChars =
            "xC0-xCF,xB0,xD1-xD6,xB7,xD8-xDC,xB5-xB6,xDF,xEF,x21-x7E,xFF";

        public const string asciiChars =
            "x00-x7F";
            
        public const string fullChars = 
            "<All>";

        public const string alphanumericChars =
            "x30-x39,x41-x5A,x61-x7A";
       
        public static string charsFromEnum (ImportCharPresetsType preset) {
            switch(preset) {
                case ImportCharPresetsType.Legacy:              return legacyChars;
                case ImportCharPresetsType.Alphanumeric:        return alphanumericChars;
                case ImportCharPresetsType.FullUnicode:         return fullChars;
                case ImportCharPresetsType.ASCII:               return asciiChars;
                default:                                        return legacyChars;
            }
        }
        
        public static ImportCharPresetsType guessEnumPresets (string s) {
            switch(s) {
                case legacyChars:                               return ImportCharPresetsType.Legacy;
                case alphanumericChars:                         return ImportCharPresetsType.Alphanumeric;
                case fullChars:                                 return ImportCharPresetsType.FullUnicode;
                case asciiChars:                                return ImportCharPresetsType.ASCII;
                default:                                        return ImportCharPresetsType.Custom;
            }
        }
        public static char[] charsFromString(string s)
        {
            if (string.IsNullOrEmpty(s))
                s = legacyChars;
            else if (s.Contains("All") || s.Contains("all") || s.Contains("ALL"))
                return new char[0];
            int pos = 0, start = 0;
            bool lastIsRange = false;
            var list = new List<char>(8);
            try
            {
                while (pos < s.Length)
                {
                    var ch = s[pos];
                    if (ch == '-' || ch == ':' || ch == ',' || ch == ';' || ch == '&' || pos == s.Length - 1)
                    {
                        string str;
                        if (pos == s.Length - 1)
                            str = s.Substring(start);
                        else
                            str = s.Substring(start, pos - start);
                        if (str.Length == 0)
                        {
                            pos++;
                            continue;
                        }
                        int parsed;
                        if (str[0] == 'x')
                            parsed = int.Parse(str.Substring(1), NumberStyles.AllowHexSpecifier);
                        else if ((str.Length > 1 && str[0] == '0' && (str[1] == 'x' || str[1] == 'X')))
                            parsed = int.Parse(str.Substring(2), NumberStyles.AllowHexSpecifier);
                        else
                            parsed = int.Parse(str);

                        parsed = parsed > 0xffff ? 0xffff : parsed;
                        if (lastIsRange)
                        {
                            for (int i = list[list.Count - 1] + 1; i < parsed; i++)
                                list.Add((char)i);
                        }
                        list.Add((char)parsed);
                        lastIsRange = ch == '-' || ch == ':';
                        pos++;
                        start = pos;
                    }
                    else if ((ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F') || ch == '.' || ch == 'x')
                        pos++;
                    else if (char.IsWhiteSpace(ch))
                        pos++;
                    else
                        break;
                }
            }
            catch (Exception) { }
            if (list.Count == 0)
                list.Add('\0');
            return list.Distinct().Take(256).ToArray();
        }
    }
}
