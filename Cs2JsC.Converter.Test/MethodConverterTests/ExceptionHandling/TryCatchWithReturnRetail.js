function RealScript__ExceptionHandlerSamples__TryCatchWithReturn(arg) {
  var stmtTemp1, tmp_;
  try {
    if (arg === 0) {
      tmp_ = RealScript__Class1__GetMoreStatic(arg);
      return tmp_;
    }
  } catch (stmtTemp1) {
    RealScript__Class1__GetMoreStatic(arg);
  }
  return arg - 1;
}
