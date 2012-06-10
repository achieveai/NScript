function RealScript__WhileLoopBlocks__DoWhilePaddedLoop(i) {
  i %= 100;
  do {
    RealScript__TmpC__Foo("{0}", System_Int32.box(i));
    --i;
  } while (i > 10);
  return i;
}
