async function TestAsyncAwait__Test8() {
  var nativeArray, tmp, tmp2;
  nativeArray = new Array(2);
  nativeArray[0] = TestAsyncAwait__Test1();
  nativeArray[1] = TestAsyncAwait__Test2();
  tmp = (await ArrayExtensions__GetAwaiter(Int32, nativeArray))[0];
  tmp2 = (await ArrayExtensions__GetAwaiter(Int32, nativeArray)).join(",").length;
  TestAsyncAwait__Compare(Int32, tmp2, 5, function TestAsyncAwait__Test8_del(a, b) {
    return a == b;
  });
  TestAsyncAwait__Compare(Int32, tmp, 12, function TestAsyncAwait__Test8_del2(a, b) {
    return a == b;
  });
}