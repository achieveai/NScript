function RealScript__WhileLoopBlocks__DoWhilePaddedLoop(i) {
  i %= 100;
  do {
    RealScript__TmpC__Foo("{0}", System__Type__BoxTypeInstance(System_Int32, i));
    --i;
  } while (i > 10);
  return i;
}