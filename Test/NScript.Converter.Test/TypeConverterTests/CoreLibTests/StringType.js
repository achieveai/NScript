String.typeId = "b";
System__String__formatHelperRegex = null;
System__String__trimStartHelperRegex = null;
System__String__trimEndHelperRegex = null;
function System__String____cctor() {
  System__String__formatHelperRegex = new RegExp("(\\{[^\\}^\\{]+\\})", "g");
  System__String__trimStartHelperRegex = new RegExp("^[\\s\\xA0]+");
  System__String__trimEndHelperRegex = new RegExp("[\\s\\xA0]+$");
};
function System__String__Compare(s1, s2) {
  s1 = s1 || "";
  s2 = s2 || "";
  if (s1 == s2)
    return 0;
  else if (s1 > s2)
    return 1;
  else
    return -1;
};
function System__String__Comparea(s1, s2, ignoreCase) {
  if (ignoreCase) {
    if (s1)
      s1 = s1.toUpperCase();
    if (s2)
      s2 = s2.toUpperCase();
  }
  return System__String__Compare(s1, s2);
};
function System__String__CompareTo(this_, s) {
  return System__String__Compare(this_, s);
};
function System__String__CompareToa(this_, s, ignoreCase) {
  return System__String__Comparea(this_, s, ignoreCase);
};
function System__String__Concat(strings) {
  return strings.innerArray.join("");
};
function System__String__Concata(s1, s2) {
  return s1 + s2;
};
function System__String__Concatb(s1, s2, s3) {
  return s1 + s2 + s3;
};
function System__String__Concatc(s1, s2, s3, s4) {
  return s1 + s2 + s3 + s4;
};
function System__String__Concatd(s1, s2) {
  return s1.toString() + s2.toString();
};
function System__String__Concate(s1, s2, s3) {
  return s1.toString() + s2.toString() + s3.toString();
};
function System__String__Concatf(s1, s2, s3, s4) {
  return s1.toString() + s2.toString() + s3.toString() + s4.toString();
};
function System__String__Concatg(strings) {
  return strings.innerArray.join("");
};
function System__String__EndsWith(this_, ch) {
  if (!this_.length)
    return this_.charCodeAt(this_.length - 1) == ch;
  return false;
};
function System__String__EndsWitha(this_, suffix) {
  if (!suffix.length)
    return true;
  if (suffix.length > this_.length)
    return false;
  return this_.substr(this_.length - suffix.length) == suffix;
};
function System__String__Equals(s1, s2, ignoreCase) {
  return System__String__Comparea(s1, s2, ignoreCase) == 0;
};
function System__String__Format(format, values) {
  return format.replace(System__String__formatHelperRegex, function(str, m) {
    var index, value; {
      index = parseInt(m.substr(1));
      value = values.get_item(index + 1);
      if (!value)
        return "";
      return value.toString();
    }
  });
};
function System__String__IndexOf(this_, ch) {
  return this_.indexOf(String.fromCharCode(ch));
};
function System__String__IndexOfa(this_, ch, startIndex) {
  return this_.indexOf(String.fromCharCode(ch), startIndex);
};
function System__String__IsNullOrEmpty(s) {
  return !s;
};
function System__String__LastIndexOf(this_, ch) {
  return this_.lastIndexOf(String.fromCharCode(ch));
};
function System__String__LastIndexOfa(this_, ch, startIndex) {
  return this_.lastIndexOf(String.fromCharCode(ch), startIndex);
};
function System__String__op_Equality(s1, s2) {
  return s1 === s2;
};
function System__String__op_Inequality(s1, s2) {
  return s1 !== s2;
};
function System__String__Split(this_, ch) {
  return System__NativeArray__GetArray(String, this_.split(String.fromCharCode(ch)));
};
function System__String__Splita(this_, str) {
  return System__NativeArray__GetArray(String, this_.split(str));
};
function System__String__Splitb(this_, ch, limit) {
  return System__NativeArray__GetArray(String, this_.split(String.fromCharCode(ch), limit));
};
function System__String__Splitc(this_, str, limit) {
  return System__NativeArray__GetArray(String, this_.split(str, limit));
};
function System__String__StartsWith(this_, ch) {
  return this_.length >= 1 && this_.charCodeAt(0) == ch;
};
function System__String__StartsWitha(this_, prefix) {
  var i;
  if (prefix.length <= this_.length) {
    for (i = 0; i < prefix.length; ++i)
      if (prefix[i] != this_[i])
        return false;
    return true;
  }
  return false;
};
function System__String__Trim(this_) {
  return System__String__TrimEnd(System__String__TrimStart(this_));
};
function System__String__TrimEnd(this_) {
  return this_.trimLeft ? this_.trimLeft() : this_.replace(System__String__trimEndHelperRegex, "");
};
function System__String__TrimStart(this_) {
  return this_.trimRight ? this_.trimRight() : this_.replace(System__String__trimStartHelperRegex, "");
};
function System__String__get_Item(this_, index) {
  return this_.charCodeAt(index);
};
String.defaultConstructor = function System_String_factory() {
  return new String();
};
System__String__EndsWith = function System__String__EndsWith(this_, ch) {
  if (!this_.length)
    return this_.charCodeAt(this_.length - 1) == ch;
  return false;
};
System__String__EndsWitha = function System__String__EndsWitha(this_, suffix) {
  if (!suffix.length)
    return true;
  if (suffix.length > this_.length)
    return false;
  return this_.substr(this_.length - suffix.length) == suffix;
};
System__String__IndexOf = function System__String__IndexOf(this_, ch) {
  return this_.indexOf(String.fromCharCode(ch));
};
System__String__IndexOfa = function System__String__IndexOfa(this_, ch, startIndex) {
  return this_.indexOf(String.fromCharCode(ch), startIndex);
};
System__String__LastIndexOf = function System__String__LastIndexOf(this_, ch) {
  return this_.lastIndexOf(String.fromCharCode(ch));
};
System__String__LastIndexOfa = function System__String__LastIndexOfa(this_, ch, startIndex) {
  return this_.lastIndexOf(String.fromCharCode(ch), startIndex);
};
System__String__TrimEnd = function System__String__TrimEnd(this_) {
  return this_.trimLeft ? this_.trimLeft() : this_.replace(System__String__trimEndHelperRegex, "");
};
System__String__TrimStart = function System__String__TrimStart(this_) {
  return this_.trimRight ? this_.trimRight() : this_.replace(System__String__trimStartHelperRegex, "");
};
System__Type__RegisterReferenceType(String, "System.String", Object, []);