//-----------------------------------------------------------------------
// <copyright file="LocalFunctionVariable.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using NScript.Utils;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for LocalFunctionVariable
    /// </summary>
    public class LocalFunctionVariable : ICustomSerializable
    {
        private readonly string _name;

        public LocalFunctionVariable(
            string name)
        {
            _name = name;
        }

        public string Name
            => _name;

        public void Serialize(ICustomSerializer serializer)
            => serializer.AddValue("name", _name);
    }
}
