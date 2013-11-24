function RealScript__TestJsonType__get_TheArray(this_) {
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  return this_.importedExtension.TheArray = this_.importedExtension.TheArray || this_.theArray ? System_ArrayG_$Int32$_.__ctor(this_.theArray) : null;
};
function RealScript__TestJsonType__set_TheArray(this_, value) {
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  this_.importedExtension.TheArray = value;
  this_.theArray = System__NativeArray__GetNativeArray(value);
};
function RealScript__TestJsonType__get_TheList(this_) {
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  return this_.importedExtension.TheList = this_.importedExtension.TheList || this_.theList ? System_Collections_Generic_List_$Int32$_.__ctor(this_.theList) : null;
};
function RealScript__TestJsonType__set_TheList(this_, value) {
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  this_.importedExtension.TheList = value;
  this_.theList = System__NativeArray__GetNativeArraya(value);
};
function RealScript__TestJsonType__get_Str(this_) {
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  return this_.importedExtension.tmpStr;
};
function RealScript__TestJsonType__set_Str(this_, value) {
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  this_.importedExtension.tmpStr = value;
};
function RealScript__TestJsonType__WorkOnString(this_) {
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  if (this_.importedExtension.tmpStr === null)
    this_.importedExtension.tmpStr = "";
  else
    this_.importedExtension.tmpStr = System__String__Concat(this_.importedExtension.tmpStr, System__Type__BoxTypeInstance(System_Int32, this_.importedExtension.tmpStr.length));
};