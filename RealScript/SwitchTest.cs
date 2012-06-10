using System;

namespace RealScript
{
    public class SwitchTest
    {
        int action;
        int changeIndex;
        object[] newItems;
        object[] oldItems;

        public void SimpleIntSwitch(int i)
        {
            string.Format("{}");

            switch (i)
            {
                case 100:
                    string.Format("OneHundred");
                    break;
                case 10:
                    string.Format("Ten");
                    break;
                case 2:
                case 16:
                case 32:
                    string.Format("Power Of Two");
                    break;
                case 101:
                    string.Format("Contigous Test");
                    break;
                case 104:
                    string.Format("Contigous Test 2");
                    break;
                case 102:
                    string.Format("Contigous Test 3");
                    break;
                case 103:
                    string.Format("Contigous Test 4");
                    break;
                default:
                    string.Format("Default");
                    break;
            }

            string.Format("{0}", "done with NoDefault stuff");
        }

        public void SimpleStringSwitch(int i)
        {
            string.Format("{}");

            switch (i.ToString())
            {
                case "100":
                    string.Format("OneHundred");
                    break;
                case "10":
                    string.Format("Ten");
                    break;
                case "101":
                    string.Format("Contigous Test");
                    break;
                case "104":
                    string.Format("Contigous Test 2");
                    break;
                case "102":
                    string.Format("Contigous Test 3");
                    break;
                case "103":
                    string.Format("Contigous Test 4");
                    break;
                default:
                    string.Format("Default");
                    break;
            }

            string.Format("{0}", "done with NoDefault stuff");
        }

        public void SwitchOnlyFunction(int i)
        {
            switch (i)
            {
                case 10:
                    string.Format("Ten");
                    break;
                case 2:
                case 16:
                case 32:
                    string.Format("Power Of Two");
                    break;
                case 103:
                    string.Format("Contigous Test 4");
                    break;
                default:
                    string.Format("Default");
                    break;
            }
        }

        public void SwitchWithReturn(int i)
        {
            string.Format("{}");

            switch (i)
            {
                case 10:
                    string.Format("Ten");
                    return;
                case 2:
                case 16:
                case 32:
                    string.Format("Power Of Two");
                    break;
                case 103:
                    string.Format("Contigous Test 4");
                    return;
                default:
                    string.Format("Default");
                    break;
            }

            string.Format("{0}", "done with NoDefault stuff");
        }

        public void SwitchWithReturnsOnly(int i)
        {
            switch (i)
            {
                case 10:
                    string.Format("Ten");
                    return;
                case 2:
                case 16:
                case 32:
                    string.Format("Power Of Two");
                    return;
                case 103:
                    string.Format("Contigous Test 4");
                    return;
                default:
                    string.Format("Default");
                    return;
            }
        }

        public void RegressionContinousSwitchValues(
            int action,
            int changeIndex,
            object[] newItems,
            object[] oldItems)
        {
            this.action = action;
            this.changeIndex = changeIndex;

            switch (action)
            {
                case 0:
                    this.newItems = newItems;
                    break;
                case 1:
                    this.oldItems = oldItems;
                    break;
                case 2:
                    this.newItems = newItems;
                    this.oldItems = oldItems;
                    break;
                case 4:
                    this.changeIndex = -1;
                    break;
            }
        }

        public void RegressSwitchWithFor(
            int action,
            int changeIndex,
            object[] newItems,
            object[] oldItems)
        {
            // We don't care about this event if the sender is not the source collection.
            // We can also ignore the change event if we are nested within other collection
            // change notification and this may be as a result of that notification.
            if (this.newItems == newItems)
            {
                return;
            }

            switch (action)
            {
                case 0:
                    this.newItems[changeIndex] = newItems[changeIndex];
                    break;
                case 1:
                    Class1.GetMoreStatic(action);
                    this.SimpleIntSwitch(oldItems.Length);
                    break;
                case 2:
                    for (int index = 0; index < newItems.Length; ++index)
                    {
                        if (this.oldItems != null)
                        {
                            this.oldItems[changeIndex + index] = newItems[index];
                        }

                        this.newItems[changeIndex + index] = newItems[index];
                    }

                    break;
                case 4:
                    Class1.GetMoreStatic(this.newItems.Length);
                    for (int index = 0; index < this.newItems.Length; ++index)
                    {
                        this.oldItems[index] = newItems[index];
                    }

                    break;
            }
        }

        static void RegressionContinousSwitch2(int arg)
        {
            Console.WriteLine("Testing switch...");
            switch (arg)
            {
                case 2:
                    Console.WriteLine("2");
                    break;
                case 3:
                    Console.WriteLine("3");
                    return;
                case 4:
                case 5:
                    Console.WriteLine("4 or 5");
                    break;
                default:
                    Console.WriteLine("default");
                    break;
            }
            Console.WriteLine("after");
        }
    }
}
