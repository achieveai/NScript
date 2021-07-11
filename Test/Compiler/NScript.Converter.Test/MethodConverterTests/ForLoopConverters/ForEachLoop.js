function ForLoopBlocks__ForEachLoop(dict) {
  var stmtTemp1a, stmtTemp1, kvPair;
  for (stmtTemp1 in(stmtTemp1a = dict)) {
    kvPair = {
      key: stmtTemp1,
      value: stmtTemp1a[stmtTemp1]
    };
    String__Format("k:{0}, v:{1}", ArrayG_$Object$_.__ctor([kvPair.key, kvPair.value]));
  }
}
