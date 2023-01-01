function NewLanguageFeatures__OutVarParam(this_, dict, str) {
  var value;
  if (dict.V_TryGetValue_b$c_d$(str, {
    rd: function() {
      return value;
    },
    wt: function(arg0) {
      return value = arg0;
    }
  }))
    return value;
  return null;
}