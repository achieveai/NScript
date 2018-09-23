function PsudoUsage() {
};
function PsudoUsage__F1(t, test) {
  return Nullable_$Int32$_.get_hasValue(TestImportedType__get_TempI(t)) ? test + Nullable_$Int32$_.get_value(TestImportedType__get_TempI(t)) : test - 1;
};
function PsudoUsage__F1a(t, test) {
  return Nullable_$Int32$_.get_hasValue(t.tempI) ? test + Nullable_$Int32$_.get_value(t.tempI) : test - 1;
};
function PsudoUsage__C1(t, i, a) {
  var stmtTemp1;
  return stmtTemp1 = new RealScript.TestImportedType(), TestImportedType__set_TempI(stmtTemp1, i === null ? null : i + t.tempJ), TestImportedType__set_TheArray(stmtTemp1, a), stmtTemp1;
};
function PsudoUsage__C1a(t, i, a) {
  var stmtTemp1;
  return stmtTemp1 = {
    tempI: i === null ? null : i + t.tempJ
  }, TestJsonType__set_TheArray(stmtTemp1, a), stmtTemp1;
};
function PsudoUsage__C2(t, i, j, k) {
  var stmtTemp1;
  return stmtTemp1 = {
    tempI: i === null ? null : i + t.tempJ
  }, TestJsonType__set_TheArray(stmtTemp1, ArrayG_$Int32$_.__ctor([j, k])), stmtTemp1;
};
function PsudoUsage__M1(t) {
  return TestImportedType__ProcessData(t, List_$Int32$_.__ctor(TestImportedType__get_TheArray(t)));
};
function PsudoUsage__TestImportedGeneric(tmp) {
  if (tmp() === null) {
    tmp.set_Something(ImportedGeneric$1__get_TheArray(tmp).get_item(0));
    return tmp(tmp.get_Something());
  }
  else if (tmp().length == 10) {
    tmp.set_Something(RealScript.ImportedGeneric.hasSomething());
    tmp(tmp.get_Something());
    ImportedGeneric$1__get_TheArray(tmp).set_item(0, tmp());
  }
  return tmp();
};
