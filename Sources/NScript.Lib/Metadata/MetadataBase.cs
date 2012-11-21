using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace NScript.Lib.MetaData
{
    public abstract class MetadataBase
    {
        #region member variables
        private List<KeyValuePair<Method, byte[]>> attributeContructors =
            new List<KeyValuePair<Method,byte[]>>();
        #endregion

        #region constructors/Factories
        protected MetadataBase()
        {
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        #endregion

        #region public functions
        protected void AddAttribute(Method ctor, byte[] arguments)
        {
            this.attributeContructors.Add(
                new KeyValuePair<Method, byte[]>(
                    ctor,
                    arguments));
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
