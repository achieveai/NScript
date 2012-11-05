function System_Int32(boxedValue) {
  this.boxedValue = boxedValue;
};
System_Int32.typeId = "b";
System_Int32.getDefaultValue = function() {
  return 0;
};
function System__Int32__Parse(s) {
  return parseInt(s);
};
function System__Int32__Parsea(s, radix) {
  return parseInt(s, radix);
};
function System__Int32__Format(this_, format) {
  return System__Int32__ToString(this_, 10);
};
function System__Int32__LocaleFormat(this_, format) {
  return System__Int32__Format(this_, format);
};
function System__Int32__ToString(this_, radix) {
  return this_.toString(radix);
};
function System__Int32__ToStringa(this_) {
  return System__Int32__ToString(this_, 10);
};
function System__Int32__ToLocaleString(this_) {
  return System__Int32__ToStringa(this_);
};
ptyp_ = new System_ValueType();
System_Int32.prototype = ptyp_;
ptyp_.toLocaleString = function() {
  return System__Int32__ToLocaleString(this.boxedValue);
};
ptyp_.toString = function() {
  return System__Int32__ToStringa(this.boxedValue);
};
System__Type__RegisterStructType(System_Int32, "System.Int32", []);