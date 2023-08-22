String.typeId = "b";
String__formatHelperRegex = null;
String__trimStartHelperRegex = null;
String__trimEndHelperRegex = null;
function String__get_FormatHelperRegex() {
  if (Object__IsNullOrUndefined(String__formatHelperRegex))
    String__formatHelperRegex = new RegExp("(\\{[^\\}^\\{]+\\})", "g");
  return String__formatHelperRegex;
}
function String__get_TrimEndHelperRegex() {
  if (Object__IsNullOrUndefined(String__trimEndHelperRegex))
    String__trimEndHelperRegex = new RegExp("^[\\s\\xA0]+");
  return String__trimEndHelperRegex;
}
function String__get_TrimStartHelperRegex() {
  if (Object__IsNullOrUndefined(String__trimStartHelperRegex))
    String__trimStartHelperRegex = new RegExp("^[\\s\\xA0]+");
  return String__trimStartHelperRegex;
}
function String__At(this_, index) {
  return this_[index];
}
function String__Compare(s1, s2) {
  s1 = s1 || "";
  s2 = s2 || "";
  if (s1 == s2)
    return 0;
  else if (s1 > s2)
    return 1;
  else
    return -1;
}
function String__Comparea(s1, s2, ignoreCase) {
  if (ignoreCase) {
    if (s1)
      s1 = s1.toUpperCase();
    if (s2)
      s2 = s2.toUpperCase();
  }
  return String__Compare(s1, s2);
}
function String__CompareTo(this_, s) {
  return String__Compare(this_, s);
}
function String__CompareToa(this_, s, ignoreCase) {
  return String__Comparea(this_, s, ignoreCase);
}
function String__Concat(strings) {
  return strings.innerArray.join("");
}
function String__Concata(s1, s2) {
  return s1 + s2;
}
function String__Concatb(s1, s2, s3) {
  return s1 + s2 + s3;
}
function String__Concatc(s1, s2, s3, s4) {
  return s1 + s2 + s3 + s4;
}
function String__Concatd(s1) {
  return s1.toString();
}
function String__Concate(s1, s2) {
  return s1.toString() + s2.toString();
}
function String__Concatf(s1, s2, s3) {
  return s1.toString() + s2.toString() + s3.toString();
}
function String__Concatg(s1, s2, s3, s4) {
  return s1.toString() + s2.toString() + s3.toString() + s4.toString();
}
function String__Concath(strings) {
  return strings.innerArray.join("");
}
function String__EndsWith(this_, ch) {
  if (!this_.length)
    return this_.charCodeAt(this_.length - 1) == ch;
  return false;
}
function String__EndsWitha(this_, suffix) {
  if (!suffix.length)
    return true;
  if (suffix.length > this_.length)
    return false;
  return this_.substr(this_.length - suffix.length) == suffix;
}
function String__Equals(s1, s2, ignoreCase) {
  return String__Comparea(s1, s2, ignoreCase) == 0;
}
function String__Format(format, values) {
  return format.replace(String__get_FormatHelperRegex(), function(str, m) {
    var index, value; {
      index = parseInt(m.substr(1));
      value = values.get_item(index);
      if (!value)
        return "";
      return value.toString();
    }
  });
}
function String__Formata(format, value) {
  return String__Format(format, ArrayG_$Object$_.__ctor([value]));
}
function String__Formatb(formatProvider, format, value) {
  return String__Format(format, value);
}
function String__IndexOf(this_, ch) {
  return this_.indexOf(String.fromCharCode(ch));
}
function String__IndexOfa(this_, ch, startIndex) {
  return this_.indexOf(String.fromCharCode(ch), startIndex);
}
function String__IsNullOrEmpty(s) {
  return !s;
}
function String__LastIndexOf(this_, ch) {
  return this_.lastIndexOf(String.fromCharCode(ch));
}
function String__LastIndexOfa(this_, ch, startIndex) {
  return this_.lastIndexOf(String.fromCharCode(ch), startIndex);
}
function String__Split(this_, ch) {
  return NativeArray__GetArray(String, this_.split(String.fromCharCode(ch)));
}
function String__Splita(this_, str) {
  return NativeArray__GetArray(String, this_.split(str));
}
function String__Splitb(this_, ch, limit) {
  return NativeArray__GetArray(String, this_.split(String.fromCharCode(ch), limit));
}
function String__Splitc(this_, str, limit) {
  return NativeArray__GetArray(String, this_.split(str, limit));
}
function String__StartsWith(this_, ch) {
  return this_.length >= 1 && this_.charCodeAt(0) == ch;
}
function String__StartsWitha(this_, prefix) {
  var i;
  if (prefix.length <= this_.length) {
    for (i = 0; i < prefix.length; ++i)
      if (prefix[i] != this_[i])
        return false;
    return true;
  }
  return false;
}
function String__Trim(this_) {
  return String__TrimEnd(String__TrimStart(this_));
}
function String__TrimEnd(this_) {
  return this_.trimLeft ? this_.trimLeft() : this_.replace(String__get_TrimEndHelperRegex(), "");
}
function String__TrimStart(this_) {
  return this_.trimRight ? this_.trimRight() : this_.replace(String__get_TrimStartHelperRegex(), "");
}
function String__get_Item(this_, index) {
  return this_.charCodeAt(index);
}
String.defaultConstructor = function System_String_factory() {
  return new String();
};
String__EndsWith = function String__EndsWith(this_, ch) {
  if (!this_.length)
    return this_.charCodeAt(this_.length - 1) == ch;
  return false;
};
String__EndsWitha = function String__EndsWitha(this_, suffix) {
  if (!suffix.length)
    return true;
  if (suffix.length > this_.length)
    return false;
  return this_.substr(this_.length - suffix.length) == suffix;
};
String__IndexOf = function String__IndexOf(this_, ch) {
  return this_.indexOf(String.fromCharCode(ch));
};
String__IndexOfa = function String__IndexOfa(this_, ch, startIndex) {
  return this_.indexOf(String.fromCharCode(ch), startIndex);
};
String__LastIndexOf = function String__LastIndexOf(this_, ch) {
  return this_.lastIndexOf(String.fromCharCode(ch));
};
String__LastIndexOfa = function String__LastIndexOfa(this_, ch, startIndex) {
  return this_.lastIndexOf(String.fromCharCode(ch), startIndex);
};
String__TrimEnd = function String__TrimEnd(this_) {
  return this_.trimLeft ? this_.trimLeft() : this_.replace(String__get_TrimEndHelperRegex(), "");
};
String__TrimStart = function String__TrimStart(this_) {
  return this_.trimRight ? this_.trimRight() : this_.replace(String__get_TrimStartHelperRegex(), "");
};
Type__RegisterReferenceType(String, "System.String", Object, []);