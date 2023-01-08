function DelegateBlocks__AddEvent(db) {
  DelegateBlocks__add_Evt(db, function DelegateBlocks__AddEvent_del(i) {
    return i + 10;
  });
}
