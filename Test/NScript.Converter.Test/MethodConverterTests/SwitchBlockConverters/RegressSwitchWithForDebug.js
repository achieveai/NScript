function SwitchTest__RegressSwitchWithFor(action, changeIndex, newItems, oldItems) {
  var index;
  if (this.newItems == newItems)
    return;
  switch(action) {
    case 0: {
      this.newItems.set_item(changeIndex, newItems.get_item(changeIndex));
      break;
    }
    case 1: {
      Class1__GetMoreStatic(action);
      this.simpleIntSwitch(oldItems.V_get_Length());
      break;
    }
    case 2: {
      for (index = 0; index < newItems.V_get_Length(); ++index) {
        if (!!this.oldItems)
          this.oldItems.set_item(changeIndex + index, newItems.get_item(index));
        this.newItems.set_item(changeIndex + index, newItems.get_item(index));
      }
      break;
    }
    case 4: {
      Class1__GetMoreStatic(this.newItems.V_get_Length());
      for (index = 0; index < this.newItems.V_get_Length(); ++index)
        this.oldItems.set_item(index, newItems.get_item(index));
      break;
    }
  }
}
