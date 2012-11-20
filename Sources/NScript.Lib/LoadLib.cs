using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NScript.Lib
{
    public class LoadLib
    {
        #region member variables
        Assembly assembly;
        List<AssemblyTypeInfo> types = new List<AssemblyTypeInfo>();
        #endregion

        #region constructors/Factories
        public LoadLib(string assemblyName)
        {
            this.assembly = Assembly.ReflectionOnlyLoadFrom(
                assemblyName);

            this.LoadReferencedAssemblies();
            this.ExtractMembers();
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        #endregion

        #region public functions
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        private void LoadReferencedAssemblies()
        {
            foreach (var reference in this.assembly.GetReferencedAssemblies())
            {
                LoadLibCache.LoadAssembly(reference, this.assembly.Location);
            }
        }

        private void ExtractMembers()
        {
            foreach (var type in this.assembly.GetTypes())
            {
                this.types.Add(
                    new AssemblyTypeInfo(
                        type));
            }
        }
        #endregion
    }
}
