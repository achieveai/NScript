function WhileLoopBlocks__WhileLoopWithBreak(i) {
  while (i > 100) {
    if (i % 22 == 0)
      return;
    if (i % 13 == 0)
      break;
    Class1__GetMoreStatic(i);
  }
  Class1__GetMoreStatic(i + 19);
}
