async function TestAsyncAwait__Test5() {
  async function LocalFunc() {
    return 1;
  }
  TestAsyncAwait__Compare(Int32, 1, (await LocalFunc()), function TestAsyncAwait__Test5_del(a, b) {
    return a == b;
  });
}