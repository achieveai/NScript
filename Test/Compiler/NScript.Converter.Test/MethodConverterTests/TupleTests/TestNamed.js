function TupleTests__TestNamed() {
  var a, b, a1, b1;
  function ReturnNamedTuple() {
    var tup, x, y, list;
    tup = TupleTests__MyDummyTuple();
    x = tup.item1, y = tup.item2, {
      item1: x,
      item2: y
    };
    x = tup.item1;
    y = tup.item2;
    list = List_$ValueTuple_$String_x_String$_$_.defaultConstructor();
    list.V_Add( {
      item1: "one",
      item2: "ln"
    });
    y = list.get_item(0).item1;
    y = list.get_item(0).item2;
    return {
      item1: x,
      item2: y
    };
  }
  a = ReturnNamedTuple().item1;
  b = ReturnNamedTuple().item2;
  a1 = ReturnNamedTuple().item1;
  b1 = ReturnNamedTuple().item2;
}