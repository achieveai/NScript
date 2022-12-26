function NewLanguageFeatures__OutVarParam(this_, dict, str) {
  var value;
  if (dict.V_TryGetValue_b$c_d$(str, {
    read: function() {
      return value;
    },
    write: function(arg0) {
      return value = arg0;
    }
  }))
    return value;
  return null;
}