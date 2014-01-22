function RealScript__ExceptionHandlerSamples__TryCatchWithReturn(arg) {
  var ex, tmp_;
  try {
    if (arg == 0) {
      tmp_ = RealScript__Class1__GetMoreStatic(arg);
      return tmp_;
    }
  } catch (ex) {
    RealScript__Class1__GetMoreStatic(arg);
  }
  tmp_ = arg - 1;
  return tmp_;
}
