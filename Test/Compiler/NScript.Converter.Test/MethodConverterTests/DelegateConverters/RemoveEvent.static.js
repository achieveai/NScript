function DelegateBlocks__RemoveEvent(db) {
  DelegateBlocks__remove_Evt(db, function DelegateBlocks__RemoveEvent_del(i) {
    return i + 10;
  });
}
