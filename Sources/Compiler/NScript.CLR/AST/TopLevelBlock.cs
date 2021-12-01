using System;
using Mono.Cecil;

namespace NScript.CLR.AST
{
    public class TopLevelBlock
    {
        /// <summary>
        /// Backing field for returnType.
        /// </summary>
        private readonly TypeReference returnType;

        /// <summary>
        /// Backing field for ownerType.
        /// </summary>
        private readonly TypeReference ownerType;

        private InlineDelegateClass delegateClass;

        /// <summary>
        /// Backing field for RootBlock.
        /// </summary>
        private ParameterBlock rootBlock;

        /// <summary>
        /// Initializes a new instance of the <see cref="TopLevelBlock"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="isStaticBlock">if set to <c>true</c> [is static block].</param>
        /// <param name="ownerType">Type of the owner.</param>
        public TopLevelBlock(
            MethodReference methodReference,
            BlockKind kind = BlockKind.Regular)
        {
            this.ownerType = methodReference.DeclaringType;
            this.returnType = methodReference.ReturnType;
            this.Kind = kind;
        }

        /// <summary>
        /// Gets the type of the return.
        /// </summary>
        /// <value>The type of the return.</value>
        public TypeReference ReturnType
        {
            get
            {
                return this.returnType;
            }
        }

        /// <summary>
        /// Gets the type of the owner.
        /// </summary>
        /// <value>The type of the owner.</value>
        public TypeReference OwnerType
        {
            get
            {
                return this.ownerType;
            }
        }

        public BlockKind Kind
        { get; private set; }

        /// <summary>
        /// Gets the root block.
        /// </summary>
        /// <value>The root block.</value>
        public ParameterBlock RootBlock
        {
            get
            {
                return this.rootBlock;
            }

            set
            {
                if (this.rootBlock == null)
                {
                    this.rootBlock = value;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// Gets the inline delegate class.
        /// </summary>
        public InlineDelegateClass InlineDelegateClass
        {
            get { return this.delegateClass; }
            internal set { this.delegateClass = value; }
        }
    }

    [Flags]
    public enum BlockKind
    {
        Regular = 1 << 0,
        Iterator = 1 << 1,
        Async = 1 << 2,
    }
}