function RealScript_PsudoUsage() {
};
function RealScript__PsudoUsage__F1(t, test) {
  return System_Nullable_$Int32$_.get_hasValue(RealScript__TestImportedType__get_TempI(t)) ? test + System_Nullable_$Int32$_.get_value(RealScript__TestImportedType__get_TempI(t)) : test - 1;
};
function RealScript__PsudoUsage__F1a(t, test) {
  return System_Nullable_$Int32$_.get_hasValue(t.tempI) ? test + System_Nullable_$Int32$_.get_value(t.tempI) : test - 1;
};
function RealScript__PsudoUsage__C1(t, i, a) {
  var stmtTemp1;
  return stmtTemp1 = new RealScript.TestImportedType(), RealScript__TestImportedType__set_TempI(stmtTemp1, i === null ? null : i + t.tempJ), stmtTemp1.theArray = System__NativeArray__GetNativeArray(a), stmtTemp1;
};
function RealScript__PsudoUsage__C1a(t, i, a) {
  var stmtTemp1;
  return {
    tempI: i === null ? null : i + t.tempJ,
    theArray: System__NativeArray__GetNativeArray(a)
  };
};
function RealScript__PsudoUsage__C2(t, i, j, k) {
  var stmtTemp1;
  return {
    tempI: i === null ? null : i + t.tempJ,
    theArray: [j, k]
  };
};
function RealScript__PsudoUsage__M1(t) {
  return RealScript__TestImportedType__ProcessData(t, System_Collections_Generic_List_$Int32$_.__ctor(RealScript__TestImportedType__get_TheArray(t)));
};
function RealScript__PsudoUsage__TestImportedGeneric(tmp) {
  if (tmp() === null) {
    tmp.set_Something(RealScript.ImportedGeneric.get_theArray(tmp).get_item(0));
    return tmp(tmp.get_Something());
  }
  else if (tmp().length === 10) {
    tmp.set_Something(RealScript.ImportedGeneric.hasSomething());
    tmp(tmp.get_Something());
    RealScript.ImportedGeneric.get_theArray(tmp).set_item(0, tmp());
  }
  return tmp();
};