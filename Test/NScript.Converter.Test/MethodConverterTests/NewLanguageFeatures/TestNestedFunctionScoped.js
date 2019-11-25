function NewLanguageFeatures__TestNestedFunctionScoped(obj) {
  function Compute(l) {
    function Compute2(l2) {
      return l2 + 10;
    };
    return obj.addNum(Compute2(l));
  };
  if (!obj) {
    function Compute2(l) {
      return l;
    };
    return Compute2(obj.addNum(10));
  }
  return Compute(obj.addNum(11));
}