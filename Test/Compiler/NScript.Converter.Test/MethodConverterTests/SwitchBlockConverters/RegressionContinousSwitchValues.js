function SwitchTest__RegressionContinousSwitchValues(action, changeIndex, newItems, oldItems) {
  this.action = action;
  this.changeIndex = changeIndex;
  switch(action) {
    case 0: {
      this.newItems = newItems;
      break;
    }
    case 1: {
      this.oldItems = oldItems;
      break;
    }
    case 2: {
      this.newItems = newItems;
      this.oldItems = oldItems;
      break;
    }
    case 4: {
      this.changeIndex = -1;
      break;
    }
  }
}
