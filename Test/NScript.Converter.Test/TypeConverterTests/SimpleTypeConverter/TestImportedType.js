function RealScript__TestImportedType__get_TempI(this_) {
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  return this_.importedExtension.TempI;
};
function RealScript__TestImportedType__set_TempI(this_, value) {
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  return this_.importedExtension.TempI = value;
};
function RealScript__TestImportedType__get_TheArray(this_) {
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  return this_.importedExtension.TheArray = this_.importedExtension.TheArray || this_.theArray ? System_ArrayG_$Int32$_.__ctor(this_.theArray) : null;
};
function RealScript__TestImportedType__set_TheArray(this_, value) {
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  this_.importedExtension.TheArray = value;
  this_.theArray = System__NativeArray__GetNativeArray(value);
};
function RealScript__TestImportedType__get_Str(this_) {
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  return this_.importedExtension.tmpStr;
};
function RealScript__TestImportedType__set_Str(this_, value) {
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  this_.importedExtension.tmpStr = value;
};
function RealScript__TestImportedType__add_TestEvent(this_, value) {
  System__EventBinder__AddEvent(this_, "testevent", value, false);
};
function RealScript__TestImportedType__remove_TestEvent(this_, value) {
  System__EventBinder__RemoveEvent(this_, "testevent", value, false);
};
function RealScript__TestImportedType__ProcessData(this_, data) {
  return System_ArrayG_$String$_.__ctor(this_.processData(System__NativeArray__GetNativeArraya(data)));
};
function RealScript__TestImportedType__WorkOnString(this_) {
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  if (this_.importedExtension.tmpStr === null)
    this_.importedExtension.tmpStr = "";
  else
    this_.importedExtension.tmpStr = System__String__Concat(this_.importedExtension.tmpStr, System_Int32.box(this_.importedExtension.tmpStr.length));
};
function RealScript__TestImportedType_factory() {
  return new RealScript.TestImportedType();
};
RealScript.TestImportedType.defaultConstructor = RealScript__TestImportedType_factory;