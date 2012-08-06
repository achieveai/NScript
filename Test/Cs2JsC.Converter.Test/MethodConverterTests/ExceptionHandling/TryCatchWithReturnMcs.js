function RealScript__ExceptionHandlerSamples__TryCatchWithReturn(arg) {
  var ex;
  try {
    if (arg === 0)
      return RealScript__Class1__GetMoreStatic(arg);
  } catch (ex) {
    RealScript__Class1__GetMoreStatic(arg);
  }
  return arg - 1;
}
