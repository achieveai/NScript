function RealScript__WhileLoopBlocks__WhileLoopWithContinue(i) {
  while (i > 100) {
    if (i % 22 === 0)
      return;
    if (i % 13 === 0)
      continue;
    RealScript__Class1__GetMoreStatic(i);
  }
  RealScript__Class1__GetMoreStatic(i + 19);
}
