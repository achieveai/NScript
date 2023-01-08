function BasicStatements__PassVariableByRef(i) {
  var j;
  j = 10;
  return BasicStatements__AccessRefArgument(i, {
    rd: function() {
      return j;
    },
    wt: function(arg0) {
      return j = arg0;
    }
  }) + j;
}
