function NewLanguageFeatures__TestNestedFunctionScoped(obj) {
  function Compute(l) {
    function Compute2a(l2) {
      return l2 + 10;
    }
    return NewLanguageFeatures__AddNum(obj, Compute2a(l));
  }
  if (!obj) {
    function Compute2(l) {
      return l;
    }
    return Compute2(NewLanguageFeatures__AddNum(obj, 10));
  }
  return Compute(NewLanguageFeatures__AddNum(obj, 11));
}