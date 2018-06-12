function SwitchTest__RegressionContinousSwitchValues(action, changeIndex, newItems, oldItems) {
  this.action = action;
  this.changeIndex = changeIndex;
  switch(action) {
    default:
    return;
    case 0: {
      this.newItems = newItems;
      return;
    }
    case 1: {
      this.oldItems = oldItems;
      return;
    }
    case 2: {
      this.newItems = newItems;
      this.oldItems = oldItems;
      return;
    }
    case 4: {
      this.changeIndex = -1;
      break;
    }
  }
}
