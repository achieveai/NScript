function RealScript__ExceptionHandlerSamples__TryFinallyWithReturn(arg) {
  var tmp_;
  try {
    if (arg === 0) {
      tmp_ = RealScript__Class1__GetMoreStatic(arg);
      return tmp_;
    }
  } finally {
    RealScript__Class1__GetMoreStatic(arg);
  }
  return arg - 1;
}
