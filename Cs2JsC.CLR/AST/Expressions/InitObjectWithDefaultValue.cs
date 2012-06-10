//-----------------------------------------------------------------------
// <copyright file="InitObjectWithDefaultValue.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;
    using Cs2JsC.Utils;


    /// <summary>
    /// Definition for InitObjectWithDefaultValue
    /// </summary>
    public class InitObjectWithDefaultValue : Expression
    {
        /// <summary>
        /// Backing field for TypeReference.
        /// </summary>
        private readonly TypeReference typeReference;

        /// <summary>
        /// Backing field for ObjectAddress.
        /// </summary>
        private LoadAddressExpression objectAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitObjectWithDefaultValue"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="paramDef">The type reference.</param>
        /// <param name="objectAddress">The object address.</param>
        public InitObjectWithDefaultValue(
            ClrContext context,
            Location location,
            TypeReference typeReference,
            LoadAddressExpression objectAddress)
            : base(context, location)
        {
            this.typeReference = typeReference;
            this.objectAddress = objectAddress;
        }

        /// <summary>
        /// Gets the type reference.
        /// </summary>
        /// <value>The type reference.</value>
        public TypeReference TypeReference
        {
            get { return this.typeReference; }
        }

        /// <summary>
        /// Gets the object reference.
        /// </summary>
        /// <value>The object reference.</value>
        public LoadAddressExpression ObjectReference
        {
            get { return this.objectAddress; }
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.objectAddress = (LoadAddressExpression)processor.Process(this.objectAddress);
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            serializationInfo.AddValue("objectAddress", this.objectAddress);
            serializationInfo.AddValue("paramDef", this.typeReference.ToString());
        }
    }
}
