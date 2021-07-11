namespace NScript.Template.Compiler
{
    using System.Collections.Generic;

    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    internal sealed partial class DefaultSettings
    {
        Dictionary<string, string> knownNamespaces = null;

        public DefaultSettings() {
            // // To add event handlers for saving and changing settings, uncomment the lines below:
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //
        }
        
        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            this.knownNamespaces = null;
        }
        
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            // Add code to handle the SettingsSaving event here.
        }

        public Dictionary<string,string> KnownNamespaces
        {
            get
            {
                if (this.knownNamespaces == null)
                {
                    this.knownNamespaces = new Dictionary<string, string>();

                    for (int iAlias = 0; iAlias < this.KnownNSAliases.Count; iAlias++)
                    {
                        string[] aliasParts = this.KnownNSAliases[iAlias].Split(':');

                        if (aliasParts.Length == 2)
                        {
                            this.knownNamespaces[aliasParts[0]] = aliasParts[1];
                        }
                    }
                }

                return this.knownNamespaces;
            }
        }
    }
}
