function Lang8Features__SwitchConstantExpression() {
  var x, y, str;
  x = 90;
  y = x === 100 ? 200 : x === 90 ? 200 : true ? 900 : null;
  str = x === 1 ? "1" : x === 2 ? "2" : true ? "10" : null;
}
