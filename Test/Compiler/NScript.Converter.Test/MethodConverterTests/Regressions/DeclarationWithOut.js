function FuncRegressions__DeclarationWithOut(dict) {
  var func;
  if (dict.tryGetValue("foo", {
    rd: function() {
      return func;
    },
    wt: function(arg0) {
      return func = arg0;
    }
  }))
    return func("foo");
  return null;
}