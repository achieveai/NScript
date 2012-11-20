namespace NScript.Template.Compiler
{
    internal static class Logger
    {
        /// <summary>
        /// Backing field for Instance.
        /// </summary>
        private static ILog instance;

        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static ILog Instance
        {
            get
            {
                return Logger.instance;
            }

            set
            {
                Logger.instance = value;
            }
        }
    }
}
