function BasicStatements__PassArrayElementByRef(intArray) {
  BasicStatements__AccessRefArgument(10, (function(arg0, arg1) {
    return {
      rd: function() {
        return arg0[arg1];
      },
      wt: function(arg0a) {
        return arg0[arg1] = arg0a;
      }
    };
  })(intArray, 10));
}

