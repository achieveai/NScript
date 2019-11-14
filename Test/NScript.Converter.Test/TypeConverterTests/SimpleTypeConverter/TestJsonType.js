function TestJsonType__get_TheArray(this_) {
  this_.importedExtension = this_.importedExtension || Object__GetNewImportedExtension();
  return this_.importedExtension.TheArray = this_.importedExtension.TheArray || (this_.theArray ? ArrayG_$Int32$_.__ctor(this_.theArray) : null);
};
function TestJsonType__set_TheArray(this_, value) {
  this_.importedExtension = this_.importedExtension || Object__GetNewImportedExtension();
  this_.importedExtension.TheArray = value;
  this_.theArray = NativeArray__GetNativeArray(value);
};
function TestJsonType__get_TheList(this_) {
  this_.importedExtension = this_.importedExtension || Object__GetNewImportedExtension();
  return this_.importedExtension.TheList = this_.importedExtension.TheList || (this_.theList ? List_$Int32$_.__ctor(this_.theList) : null);
};
function TestJsonType__set_TheList(this_, value) {
  this_.importedExtension = this_.importedExtension || Object__GetNewImportedExtension();
  this_.importedExtension.TheList = value;
  this_.theList = NativeArray__GetNativeArraya(value);
};
function TestJsonType__get_Str(this_) {
  this_.importedExtension = this_.importedExtension || Object__GetNewImportedExtension();
  return this_.importedExtension.tmpStr;
};
function TestJsonType__set_Str(this_, value) {
  this_.importedExtension = this_.importedExtension || Object__GetNewImportedExtension();
  this_.importedExtension.tmpStr = value;
};
function TestJsonType__WorkOnString(this_) {
  this_.importedExtension = this_.importedExtension || Object__GetNewImportedExtension();
  if (this_.importedExtension.tmpStr == null)
    this_.importedExtension.tmpStr = "";
  else
    this_.importedExtension.tmpStr = this_.importedExtension.tmpStr + Type__BoxTypeInstance(Int32, this_.importedExtension.tmpStr.length);
};
