function FuncRegressions__CollapsingForInIfRegression(headerPair) {
  var request, iHeader;
  request = MyDictionary_$String_x_String$_.defaultConstructor();
  if (!!headerPair)
    for (iHeader = 0; iHeader < headerPair.gl() - 1; iHeader += 2)
      request.add(headerPair.get_item(iHeader), headerPair.get_item(iHeader + 1));
}