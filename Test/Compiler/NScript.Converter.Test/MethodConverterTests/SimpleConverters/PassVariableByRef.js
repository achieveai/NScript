function BasicStatements__PassVariableByRef(i) {
  var j;
  j = 10;
  return BasicStatements__AccessRefArgument(i, {
    read: function() {
      return j;
    },
    write: function(arg0) {
      return j = arg0;
    }
  }) + j;
}
