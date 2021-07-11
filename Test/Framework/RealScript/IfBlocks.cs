//-----------------------------------------------------------------------
// <copyright file="BasicBlocks.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Definition for BasicBlocks
    /// </summary>
    public class IfBlocks
    {
        public int IfBlock(int i)
        {
            if (i > 10)
            {
                i = i - 10;
            }

            return i;
        }

        public int IfElseBlock(int i)
        {
            if (i > 10)
            {
                i = i - 10;
            }
            else
            {
                i = i + 1;
            }

            return i;
        }

        public int NestedIfElseBlock(int i)
        {
            if (i > 10)
            {
                if (i > 100)
                {
                    i = i - 50;
                }
                else
                {
                    i = i - 10;
                }
            }
            else
            {
                i = i + 1;
            }

            return i;
        }

        public int IfNestedElseBlock(int i)
        {
            if (i > 10)
            {
                i = i + 1;
            }
            else
            {
                if (i > 100)
                {
                    i = i - 50;
                }
                else
                {
                    i = i - 10;
                }
            }

            return i;
        }

        public int IfReturnBlock(int i)
        {
            if (i > 10)
            {
                return i - 10;
            }

            return i + 10;
        }
    }
}
