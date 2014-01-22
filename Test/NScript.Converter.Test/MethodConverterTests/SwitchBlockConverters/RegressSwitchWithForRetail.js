function RealScript__SwitchTest__RegressSwitchWithFor(action, changeIndex, newItems, oldItems) {
  var index;
  if (this.newItems == newItems)
    return;
  switch(action) {
    case 0: {
      this.newItems.set_item(changeIndex, newItems.get_item(changeIndex));
      return;
    }
    case 1: {
      RealScript__Class1__GetMoreStatic(action);
      this.simpleIntSwitch(oldItems.V_get_Length());
      return;
    }
    case 2: {
      for (index = 0; index < newItems.V_get_Length(); ++index) {
        if (this.oldItems !== null)
          this.oldItems.set_item(changeIndex + index, newItems.get_item(index));
        this.newItems.set_item(changeIndex + index, newItems.get_item(index));
      }
      return;
    }
    case 4: {
      RealScript__Class1__GetMoreStatic(this.newItems.V_get_Length());
      for (index = 0; index < this.newItems.V_get_Length(); ++index)
        this.oldItems.set_item(index, newItems.get_item(index));
      break;
    }
    case 3:
    return;
  }
}
