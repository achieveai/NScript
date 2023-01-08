function TestGenericB() {
};
TestGenericB.typeId = "b";
TestGenericB__theA = null;
function TestGenericB____cctor() {
  TestGenericB__theA = TestGenericA_$TestGenericB$_.defaultConstructor();
};
function TestGenericB__M(this_, a) {
  return 2;
};
function TestGenericB____ctor(this_) {
};
function TestGenericB_factory() {
  var this_;
  this_ = new TestGenericB();
  TestGenericB____ctor(this_);
  return this_;
};
TestGenericB.defaultConstructor = TestGenericB_factory;
ptyp_ = TestGenericB.prototype;
ptyp_.V_M_c$d$b$$ = function(a) {
  return TestGenericB__M(this, a);
};
Type__RegisterReferenceType(TestGenericB, "RealScript.TestGenericB", Object, [IX_$TestGenericA_$TestGenericB$_$_]);
