function WhileLoopBlocks__WhilePaddedLoop(i) {
  i %= 100;
  while (i > 10) {
    TmpC__Foo("{0}", Type__BoxTypeInstance(Int32, i));
    --i;
  }
  return i;
}
