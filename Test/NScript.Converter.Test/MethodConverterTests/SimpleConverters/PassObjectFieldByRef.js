function BasicStatements__PassObjectFieldByRef(cl) {
  BasicStatements__AccessRefArgument(10, (function(arg0) {
    return {
      read: function() {
        return arg0.intField;
      },
      write: function(arg0a) {
        return arg0.intField = arg0a;
      }
    };
  })(cl));
}
