function TestImportedType__get_TempI(this_) {
  this_.importedExtension = this_.importedExtension || Object__GetNewImportedExtension();
  return this_.importedExtension._$TempI$_k__BackingField;
};
function TestImportedType__set_TempI(this_, value) {
  this_.importedExtension = this_.importedExtension || Object__GetNewImportedExtension();
  this_.importedExtension._$TempI$_k__BackingField = value;
};
function TestImportedType__get_TheArray(this_) {
  this_.importedExtension = this_.importedExtension || Object__GetNewImportedExtension();
  return this_.importedExtension.TheArray = this_.importedExtension.TheArray || (this_.theArray ? ArrayG_$Int32$_.__ctor(this_.theArray) : null);
};
function TestImportedType__set_TheArray(this_, value) {
  this_.importedExtension = this_.importedExtension || Object__GetNewImportedExtension();
  this_.importedExtension.TheArray = value;
  this_.theArray = NativeArray__GetNativeArray(value);
};
function TestImportedType__get_Str(this_) {
  this_.importedExtension = this_.importedExtension || Object__GetNewImportedExtension();
  return this_.importedExtension.tmpStr;
};
function TestImportedType__set_Str(this_, value) {
  this_.importedExtension = this_.importedExtension || Object__GetNewImportedExtension();
  this_.importedExtension.tmpStr = value;
};
function TestImportedType__add_TestEvent(this_, value) {
  EventBinder__AddEvent(this_, "testevent", value, false);
};
function TestImportedType__remove_TestEvent(this_, value) {
  EventBinder__RemoveEvent(this_, "testevent", value, false);
};
function TestImportedType__ProcessData(this_, data) {
  return ArrayG_$String$_.__ctor(this_.processData(NativeArray__GetNativeArraya(data)));
};
function TestImportedType__WorkOnString(this_) {
  this_.importedExtension = this_.importedExtension || Object__GetNewImportedExtension();
  if (this_.importedExtension.tmpStr === null)
    this_.importedExtension.tmpStr = "";
  else
    this_.importedExtension.tmpStr = this_.importedExtension.tmpStr + Type__BoxTypeInstance(Int32, this_.importedExtension.tmpStr.length);
};
function TestImportedType_factory() {
  return new RealScript.TestImportedType();
};
RealScript.TestImportedType.defaultConstructor = TestImportedType_factory;
