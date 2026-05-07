//-----------------------------------------------------------------------
// <copyright file="ArrayWithSpreadsInitialization.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using NScript.Utils;
    using Mono.Cecil;

    /// <summary>
    /// Spread-aware array construction for C# 12 collection expressions
    /// targeting <c>T[]</c>. Holds an ordered list of items where each item
    /// is either a literal element (treated as a single value) or a spread
    /// element (treated as a flattenable array source). Lowered by the
    /// converter into a JS <c>Array.prototype.concat</c> chain.
    ///
    /// Phase F1 restricts spread sources to <c>T[]</c> (validated at Stage 1).
    /// </summary>
    public class ArrayWithSpreadsInitialization : Expression
    {
        private readonly TypeReference elementType;
        private readonly TypeReference resultType;
        private readonly List<ArrayInitializationItem> items;
        private readonly ReadOnlyCollection<ArrayInitializationItem> readonlyItems;

        public ArrayWithSpreadsInitialization(
            ClrContext context,
            Location location,
            TypeReference elementType,
            IList<ArrayInitializationItem> items)
            : base(context, location)
        {
            this.elementType = elementType;
            this.resultType = new ArrayType(this.elementType);
            this.items = new List<ArrayInitializationItem>(items);
            this.readonlyItems = new ReadOnlyCollection<ArrayInitializationItem>(this.items);
        }

        public TypeReference ElementType
        {
            get { return this.elementType; }
        }

        public IList<ArrayInitializationItem> Items
        {
            get { return this.readonlyItems; }
        }

        public override TypeReference ResultType
        {
            get { return this.resultType; }
        }

        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            serializationInfo.AddValue("elementType", this.elementType.FullName);
            serializationInfo.AddValue(
                "items",
                this.items,
                (s, item) =>
                {
                    s.AddValue("isSpread", item.IsSpread);
                    s.AddValue("operand", item.Operand);
                });
        }

        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            for (int i = 0; i < this.items.Count; i++)
            {
                var current = this.items[i];
                this.items[i] = new ArrayInitializationItem(
                    current.IsSpread,
                    (Expression)processor.Process(current.Operand));
            }
        }

        public override bool Equals(object obj)
        {
            ArrayWithSpreadsInitialization right = obj as ArrayWithSpreadsInitialization;

            if (right == null
                || !this.ResultType.Equals(right.ResultType)
                || this.items.Count != right.items.Count)
            {
                return false;
            }

            for (int i = 0; i < this.items.Count; i++)
            {
                if (this.items[i].IsSpread != right.items[i].IsSpread
                    || !this.items[i].Operand.Equals(right.items[i].Operand))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int rv = typeof(ArrayWithSpreadsInitialization).GetHashCode()
                ^ this.elementType.GetHashCode();

            foreach (var item in this.items)
            {
                rv ^= item.Operand.GetHashCode();
                if (item.IsSpread)
                {
                    rv ^= 0x5A5A5A5A;
                }
            }

            return rv;
        }
    }

    /// <summary>
    /// One element inside <see cref="ArrayWithSpreadsInitialization"/>:
    /// either a literal value or a spread source.
    /// </summary>
    public readonly struct ArrayInitializationItem
    {
        public ArrayInitializationItem(bool isSpread, Expression operand)
        {
            this.IsSpread = isSpread;
            this.Operand = operand;
        }

        public bool IsSpread { get; }

        public Expression Operand { get; }
    }
}
