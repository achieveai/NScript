//-----------------------------------------------------------------------
// <copyright file="Lan8Features.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Definition for Lan8Features
    /// </summary>
    public class Lang8Features
    {
        public static void NullCoalescingAssignment()
        {
            (int, int)? tupl = null;
            tupl ??= (1, 2);

            _ = (tupl ??= (3, 4)).Item1;
        }

        public static void IsPatternExpression()
        {
            var x = 90;
            var t = x is 900;
            var asdf = x is int z;

            if (x is 900)
            {
            }
            else if (x is int y)
            {
            }

            BaseClass b = new SubClass();
            if (b is SubClass sb)
            {
            }
        }

        public static void SwitchExpression()
        {
            var x = 90;
            var y = (x) switch
            {
                100 => 200,
                90 => 200,
                _ => 900
            };

            var str = x switch
            {
                1 => "1",
                2 => "2",
                _ => "10"
            };
        }

        internal enum Direction
        {
            Up,
            Down,
            Right,
            Left
        }

        internal enum Orientation
        {
            North,
            South,
            East,
            West
        }

        internal static Orientation ToOrientation(Direction direction) => direction switch
        {
            Direction.Up    => Orientation.North,
            Direction.Right => Orientation.East,
            Direction.Down  => Orientation.South,
            _ => Orientation.West,
        };

        private class BaseClass
        {
            public int X { get; set; }
        }

        private class SubClass : BaseClass
        {
            public new int X { get; set; }
        }
    }
}
