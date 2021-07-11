function BasicStatements__PassArrayElementByRef(intArray) {
  BasicStatements__AccessRefArgument(10, (function(arg0, arg1) {
    return {
      read: function() {
        return arg0[arg1];
      },
      write: function(arg0a) {
        return arg0[arg1] = arg0a;
      }
    };
  })(intArray, 10));
}

