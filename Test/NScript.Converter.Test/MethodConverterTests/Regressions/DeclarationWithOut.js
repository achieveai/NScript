function FuncRegressions__DeclarationWithOut(dict) {
  var func;
  if (dict.tryGetValue("foo", {
    read: function() {
      return func;
    },
    write: function(arg0) {
      return func = arg0;
    }
  }))
    return func("foo");
  return null;
}