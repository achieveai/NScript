function NewLanguageFeatures__TestNestedFunctionCrossReferenced(obj) {
  function Compute(l) {
    if (l < 10)
      l = Compute2(l, 10);
    return NewLanguageFeatures__AddNum(obj, l);
  }
  function Compute2(l, ll) {
    return NewLanguageFeatures__AddNum(obj, l) * ll;
  }
  return function NewLanguageFeatures__TestNestedFunctionCrossReferenced_del(x, y) {
    return x + Compute(y);
  };
}