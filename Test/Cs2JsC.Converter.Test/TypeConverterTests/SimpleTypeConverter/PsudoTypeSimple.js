function RealScript__TestPsudoType__get_TheArray(this_) {
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  return this_.importedExtension.TheArray = this_.importedExtension.TheArray || this_.theArray ? System_ArrayG_$Int32$_.__ctor(this_.theArray) : null;
};
function RealScript__TestPsudoType__set_TheArray(this_, value) {
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  this_.importedExtension.TheArray = value;
  this_.theArray = System__NativeArray__GetNativeArray(value);
};
function RealScript__TestPsudoType__add_TestEvent(this_, value) {
  System__EventBinder__AddEvent(this_, "testevent", value, false);
};
function RealScript__TestPsudoType__remove_TestEvent(this_, value) {
  System__EventBinder__RemoveEvent(this_, "testevent", value, false);
};
function RealScript__TestPsudoType__WorkOnString(this_) {
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  if (this_.importedExtension.tmpStr === null)
    this_.importedExtension.tmpStr = "";
  else
    this_.importedExtension.tmpStr = System__String__Concat(this_.importedExtension.tmpStr, System_Int32.box(this_.importedExtension.tmpStr.length));
};

