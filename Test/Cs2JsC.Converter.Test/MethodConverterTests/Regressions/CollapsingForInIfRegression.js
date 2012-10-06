function RealScript__FuncRegressions__CollapsingForInIfRegression(headerPair) {
  var request, iHeader;
  request = RealScript_MyDictionary_$String_x_String$_.defaultConstructor();
  if (headerPair !== null)
    for (iHeader = 0; iHeader < headerPair.V_get_Length() - 1; iHeader += 2)
      request.add(headerPair.get_item(iHeader), headerPair.get_item(iHeader + 1));
}