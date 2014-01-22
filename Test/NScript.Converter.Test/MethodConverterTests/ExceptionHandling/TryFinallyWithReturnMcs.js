function RealScript__ExceptionHandlerSamples__TryFinallyWithReturn(arg) {
  try {
    if (arg == 0)
      return RealScript__Class1__GetMoreStatic(arg);
  } finally {
    RealScript__Class1__GetMoreStatic(arg);
  }
  return arg - 1;
}
