function DelegateBlocks__CallEvent(db) {
  if (!!db.evt)
    return db.evt(10);
  return -1;
}