function NewLanguageFeatures__TestNestedFunction(obj) {
  function Compute(l) {
    return obj.addNum(l);
  }
  return function NewLanguageFeatures__TestNestedFunction_del(x, y) {
    return x + Compute(y);
  };
}