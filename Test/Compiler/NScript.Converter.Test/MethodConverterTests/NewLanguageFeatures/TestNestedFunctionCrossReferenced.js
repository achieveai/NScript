function NewLanguageFeatures__TestNestedFunctionCrossReferenced(obj) {
  function Compute(l) {
    if (l < 10)
      l = Compute2(l, 10);
    return obj.addNum(l);
  };
  function Compute2(l, ll) {
    return obj.addNum(l) * ll;
  };
  return function NewLanguageFeatures__TestNestedFunctionCrossReferenced_del(x, y) {
    return x + Compute(y);
  };
}