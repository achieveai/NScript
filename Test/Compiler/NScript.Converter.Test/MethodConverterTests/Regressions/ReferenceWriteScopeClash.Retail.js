function a(a, b) {
  c( {
    rd: function() {
      return a;
    },
    wt: function(b) {
      return a = b;
    }
  }, {
    rd: function() {
      return b;
    },
    wt: function(a) {
      return b = a;
    }
  });
}