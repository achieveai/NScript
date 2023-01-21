function TestGenericB() { }
TestGenericB.typeId = "b";
TestGenericB__theA = null;
function TestGenericB____cctor() {
  TestGenericB__theA = TestGenericA_$TestGenericB$_.defaultConstructor();
}
function TestGenericB_factory() {
  var this_;
  this_ = new TestGenericB();
  this_.__ctor();
  return this_;
}
TestGenericB.defaultConstructor = TestGenericB_factory;
ptyp_ = TestGenericB.prototype;
ptyp_.m = function TestGenericB__M(a) {
  return 2;
};
ptyp_.__ctor = function TestGenericB____ctor() { };
ptyp_.V_M_c$d$b$$ = ptyp_.m;
Type__RegisterReferenceType(TestGenericB, "RealScript.TestGenericB", Object, [IX_$TestGenericA_$TestGenericB$_$_]);