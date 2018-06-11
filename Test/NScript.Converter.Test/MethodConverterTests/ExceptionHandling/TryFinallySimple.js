function ExceptionHandlerSamples__TryFinallySimple(arg) {
  arg = Class1__GetMoreStatic(arg);
  try {
    arg = Class1__GetMoreStatic(arg);
  } finally {
    Class1__GetMoreStatic(arg);
  }
  return arg - 10;
}
