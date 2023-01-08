function BasicStatements__TestRefs_a(a_a, b_b) {
  TestControlFlow__Swap_c( {
    rd: function() {
      return a_a;
    },
    wt: function(arg0_b) {
      return a_a = arg0_b;
    }
  }, {
    rd: function() {
      return b_b;
    },
    wt: function(arg0_a) {
      return b_b = arg0_a;
    }
  });
}
