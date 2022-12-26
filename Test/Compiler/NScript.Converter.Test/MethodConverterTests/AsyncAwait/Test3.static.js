async function TestAsyncAwait__Test3() {
  var cls, nativeArray, tmp;
  Console__WriteLine("Test3");
  cls = MyClass_factory(TestAsyncAwait__Test2());
  nativeArray = new Array(2);
  nativeArray[0] = TestAsyncAwait__Test1();
  nativeArray[1] = TestAsyncAwait__Test2();
  tmp = await ArrayExtensions__GetAwaiter(Int32, nativeArray);
  return Int32__ToString((await MyClass__GetAwaiter(cls))) + tmp;
}