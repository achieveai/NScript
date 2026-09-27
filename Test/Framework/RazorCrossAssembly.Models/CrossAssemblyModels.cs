namespace RazorCrossAssembly.Models
{
    using Sunlight.Framework.Observables;

    public class CrossAssemblyChildModel : ObservableObject
    {
        private string name;

        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.FirePropertyChanged("Name");
                }
            }
        }
    }

    public class CrossAssemblyParentModel : ObservableObject
    {
        private string title;
        private CrossAssemblyChildModel child;

        public string Title
        {
            get { return this.title; }
            set
            {
                if (this.title != value)
                {
                    this.title = value;
                    this.FirePropertyChanged("Title");
                }
            }
        }

        public CrossAssemblyChildModel Child
        {
            get { return this.child; }
            set
            {
                if (this.child != value)
                {
                    this.child = value;
                    this.FirePropertyChanged("Child");
                }
            }
        }
    }
}
