function WhileLoopBlocks__DoWhilePaddedLoop(i) {
  i %= 100;
  do {
    TmpC__Foo("{0}", Type__BoxTypeInstance(Int32, i));
    --i;
  } while (i > 10);
  return i;
}