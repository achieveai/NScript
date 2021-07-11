function ExceptionHandlerSamples__TryCatchWithReturn(arg) {
  var ex;
  try {
    if (arg == 0)
      return Class1__GetMoreStatic(arg);
  } catch (ex) {
    Class1__GetMoreStatic(arg);
    return 0;
  }
  return arg - 1;
}
