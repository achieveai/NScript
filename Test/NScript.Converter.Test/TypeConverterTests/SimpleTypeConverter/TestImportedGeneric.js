function ImportedGeneric$1__get_TheArray(this_) {
  this_.importedExtension = this_.importedExtension || Object__GetNewImportedExtension();
  return this_.importedExtension.TheArray = this_.importedExtension.TheArray || (this_.theArray ? ArrayG$1_$T$_.__ctor(this_.theArray) : null);
};
function ImportedGeneric$1__set_TheArray(this_, value) {
  this_.importedExtension = this_.importedExtension || Object__GetNewImportedExtension();
  this_.importedExtension.TheArray = value;
  this_.theArray = NativeArray__GetNativeArray(value);
};
RealScript.ImportedGeneric.defaultConstructor = function RealScript_ImportedGeneric$1_factory() {
  return new RealScript.ImportedGeneric();
};
