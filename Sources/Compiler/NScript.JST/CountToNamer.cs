namespace NScript.JST
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    internal class CountToNamer
    {
        const string FirstCharMap = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string CharMap = FirstCharMap + "0123456789";

        private static readonly ImmutableArray<string> Keywords = new[]
            {
                // Keywords
                "abstract", "arguments",    "await",    "boolean",
                "break",    "byte", "case", "catch",
                "char", "class",    "const",    "continue",
                "debugger", "default",  "delete",   "do",
                "double",   "else", "enum", "eval",
                "export",   "extends",  "false",    "final",
                "finally",  "float",    "for",  "function",
                "goto", "if",   "implements",   "import",
                "in",   "instanceof",   "int",  "interface",
                "let",  "long", "native",   "new",
                "null", "package",  "private",  "protected",
                "public",   "return",   "short",    "static",
                "super",    "switch",   "synchronized", "this",
                "throw",    "throws",   "transient",    "true",
                "try",  "typeof",   "var",  "void",
                "volatile",    "while",   "with",    "yield",
                
                // Base methods / types
                "Array",    "Date", "eval", "function",
                "hasOwnProperty",   "Infinity", "isFinite", "isNaN",
                "isPrototypeOf",    "length",   "Math", "NaN",
                "name", "Number",   "Object",   "prototype",
                "String",   "toString", "undefined",    "valueOf",
                
                // Other reserved words
                "alert",    "all",  "anchor",   "anchors",
                "area", "assign",   "blur", "button",
                "checkbox", "clearInterval",    "clearTimeout", "clientInformation",
                "close",    "closed",   "confirm",  "constructor",
                "crypto",   "decodeURI",    "decodeURIComponent",   "defaultStatus",
                "document", "element",  "elements", "embed",
                "embeds",   "encodeURI",    "encodeURIComponent",   "escape",
                "event",    "fileUpload",   "focus",    "form",
                "forms",    "frame",    "innerHeight",  "innerWidth",
                "layer",    "layers",   "link", "location",
                "mimeTypes",    "navigate", "navigator",    "frames",
                "frameRate",    "hidden",   "history",  "image",
                "images",   "offscreenBuffering",   "open", "opener",
                "option",   "outerHeight",  "outerWidth",   "packages",
                "pageXOffset",  "pageYOffset",  "parent",   "parseFloat",
                "parseInt", "password", "pkcs11",   "plugin",
                "prompt",   "propertyIsEnum",   "radio",    "reset",
                "screenX",  "screenY",  "scroll",   "secure",
                "select",   "self", "setInterval",  "setTimeout",
                "status",   "submit",   "taint",    "text",
                "textarea", "top",  "unescape", "untaint",
                "window",
            }.ToImmutableArray();

        private static readonly ImmutableArray<int> CharToIdx = RmapCharMap();

        private readonly ImmutableArray<int> ReservedNamesUsed;

        private readonly ImmutableHashSet<string> ReservedNames;

        public CountToNamer(IEnumerable<string> reservedNames)
        {
            ReservedNames = Keywords
                .Concat(reservedNames)
                .ToImmutableHashSet();

            ReservedNamesUsed = ReservedNames
                .Select(NameToIntId)
                .Where(id => id >= 0)
                .OrderBy(id => id)
                .ToImmutableArray();
        }

        public static int NameToIntId(string name)
        {
            int rv = 0;
            int charRange = 1;
            for (int idx = 1; idx < name.Length; idx++)
            {
                rv *= CharMap.Length;
                charRange *= CharMap.Length;

                if (name[idx] > 127
                    || CharToIdx[name[idx]] < 0)
                {
                    return -1;
                }

                rv += CharToIdx[name[idx]];

                if (rv < 0)
                {
                    return -1;
                }
            }

            var leadingNum = CharToIdx[name[0]];
            var firstCharRange = 1;
            while(firstCharRange < charRange) {
                firstCharRange *= FirstCharMap.Length;
            }

            return rv + (leadingNum * firstCharRange);
        }

        public string IntIdToName(int id)
        {
            var idx = ReservedNamesUsed.BinarySearch(id);
            if (idx < 0)
            {
                idx = ~idx;
            }

            if (idx != 0 || ReservedNamesUsed[idx] <= id)
            { idx++; }

            // Return accounting for all the keywords that
            // we can't assign to.
            return IntIdToNameInternal(id + idx);
        }

        private static string IntIdToNameInternal(int id)
        {
            int bigMod = FirstCharMap.Length;

            while(id >= bigMod) {
                bigMod *= FirstCharMap.Length;
            }

            List<char> chars = new();
            bigMod /= FirstCharMap.Length;
            var modChar = 1;
            while (modChar < bigMod) {
                modChar *= CharMap.Length;
            }

            var modRemainder = id % bigMod;
            while(modChar > 1)
            {
                chars.Add(CharMap[modRemainder % CharMap.Length]);
                modChar /= CharMap.Length;
                modRemainder /= CharMap.Length;
            }

            chars.Add(FirstCharMap[id / bigMod]);
            chars.Reverse();
            return new string(chars.ToArray());
        }

        private static ImmutableArray<int> RmapCharMap()
        {
            int[] rv = Enumerable.Range(0, 128).Select(_ => -1).ToArray();
            for (int idx = 0; idx < CharMap.Length; idx++)
            {
                rv[CharMap[idx]] = idx;
            }

            return rv.ToImmutableArray();
        }
    }
}
