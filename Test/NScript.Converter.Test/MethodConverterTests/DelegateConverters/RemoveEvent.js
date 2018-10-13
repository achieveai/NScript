function DelegateBlocks__RemoveEvent(db) {
  db.remove_Evt(function DelegateBlocks__RemoveEvent_del(i) {
    return i + 10;
  });
}
