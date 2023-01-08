function NewLanguageFeatures__TestNestedFunction(obj) {
  function Compute(l) {
    return NewLanguageFeatures__AddNum(obj, l);
  };
  return function NewLanguageFeatures__TestNestedFunction_del(x, y) {
    return x + Compute(y);
  };
}