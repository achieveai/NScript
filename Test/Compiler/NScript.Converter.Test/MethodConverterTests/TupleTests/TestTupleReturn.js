function TupleTests__TestTupleReturn(x, y) {
  return {
    item1: {
      item1: x,
      item2: {
        item1: y,
        item2: x
      }
    },
    item2: x
  };
}