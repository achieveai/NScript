function ExceptionHandlerSamples__TryFinallyWithReturn(arg) {
  try {
    if (arg == 0)
      return Class1__GetMoreStatic(arg);
  } finally {
    Class1__GetMoreStatic(arg);
  }
  return arg - 1;
}
