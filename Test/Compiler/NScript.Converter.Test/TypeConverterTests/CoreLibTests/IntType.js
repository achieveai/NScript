    function Int32(boxedValue) {
  this.boxedValue = boxedValue;
}
Int32.typeId = "b";
Int32.getDefaultValue = function() {
  return 0;
};
function Int32__Parse(s) {
  return parseInt(s);
}
function Int32__Parsea(s, radix) {
  return parseInt(s, radix);
}
function Int32__Format(this_, format) {
  return Int32__ToString(this_, 10);
}
function Int32__LocaleFormat(this_, format) {
  return Int32__Format(this_, format);
}
function Int32__ToString(this_, radix) {
  return this_.toString(radix);
}
function Int32__ToStringa(this_) {
  return Int32__ToString(this_, 10);
}
function Int32__ToLocaleString(this_) {
  return Int32__ToStringa(this_);
}
ptyp_ = new ValueType();
Int32.prototype = ptyp_;
ptyp_.toLocaleString = function() {
  return Int32__ToLocaleString(this.boxedValue);
};
ptyp_.toString = function() {
  return Int32__ToStringa(this.boxedValue);
};
Type__RegisterStructType(Int32, "System.Int32", []);