function FuncRegressions__RegressionWithLastIndexOfString(str) {
  if (String__LastIndexOf(str, "/") > 0)
    return str + "10";
  return str;
}