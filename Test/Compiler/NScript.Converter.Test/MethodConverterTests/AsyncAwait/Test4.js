async function TestAsyncAwait__Test4() {
  var func;
  func = async function TestAsyncAwait__Test4_del() {
    return 1;
  };
  TestAsyncAwait__Compare(Int32, 1, (await func()), function TestAsyncAwait__Test4_del2(a, b) {
    return a == b;
  });
}