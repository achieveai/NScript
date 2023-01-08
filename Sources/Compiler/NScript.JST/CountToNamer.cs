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
                "abstract", "arguments", "await", "async", "boolean",
                "break",    "byte", "case", "catch",
                "char", "class",    "const",    "continue",
                "debugger", "default",  "delete",   "do",
                "double",   "else", "enum", "eval",
                "export",   "extends",  "false",    "final",
                "finally",  "float",    "for",  "function",
                "goto", "if", "in",   "implements",   "import",
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

        public string IntIdToName(int id)
        {
            int idx = 0, prev_idx = 0;
            do{
                prev_idx = idx;
                idx = ReservedNamesUsed.BinarySearch(id + prev_idx);
                if (idx < 0)
                {
                    idx = ~idx;
                }

                if (ReservedNamesUsed[idx] <= id + prev_idx)
                { idx++; }

            } while(prev_idx < idx);

            // Return accounting for all the keywords that
            // we can't assign to.
            var rv = IntIdToNameInternal(id + idx);
            if (rv == "in")
            {
            }

            return rv;
        }


        public static int NameToIntId(string str)
        {
            int rv;
            int idx;

            rv = FirstCharMap.IndexOf(str[0]);
            idx = 1;

            while (idx < str.Length) {
                rv *= CharMap.Length;
                rv += CharMap.IndexOf(str[idx]);
                idx++;
            }

            return rv;
        }

        public static string IntIdToNameInternal(int id)
        {
            List<char> chars = new();
            while(id >= FirstCharMap.Length) {
                chars.Add(CharMap[id % CharMap.Length]);
                id /= CharMap.Length;
            }

            if (id < FirstCharMap.Length) {
                chars.Add(FirstCharMap[id]);
            }

            chars.Reverse();
            return new String(chars.ToArray());
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
