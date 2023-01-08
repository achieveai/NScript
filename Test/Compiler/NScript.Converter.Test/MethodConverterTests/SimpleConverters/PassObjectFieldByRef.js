function BasicStatements__PassObjectFieldByRef(cl) {
  BasicStatements__AccessRefArgument(10, (function(arg0) {
    return {
      rd: function() {
        return arg0.intField;
      },
      wt: function(arg0a) {
        return arg0.intField = arg0a;
      }
    };
  })(cl));
}
