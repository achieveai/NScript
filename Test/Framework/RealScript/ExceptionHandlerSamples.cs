//-----------------------------------------------------------------------
// <copyright file="ExceptionHandlerSamples.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Definition for ExceptionHandlerSamples
    /// </summary>
    public static class ExceptionHandlerSamples
    {
        public static int TryFinallySimple(int arg)
        {
            arg = Class1.GetMoreStatic(arg);

            try
            {
                arg = Class1.GetMoreStatic(arg);
            }
            finally
            {
                Class1.GetMoreStatic(arg);
            }

            return arg - 10;
        }

        public static int TryFinallyWithReturn(int arg)
        {
            try
            {
                if (arg == 0)
                {
                    return Class1.GetMoreStatic(arg);
                }
            }
            finally
            {
                Class1.GetMoreStatic(arg);
            }

            return arg - 1;
        }

        public static int TryCatchSimple(int arg)
        {
            arg = Class1.GetMoreStatic(arg);

            try
            {
                arg = Class1.GetMoreStatic(arg);
            }
            catch(Exception)
            {
                Class1.GetMoreStatic(arg);
            }

            return arg - 10;
        }

        public static int TryCatchWithReturn(int arg)
        {
            try
            {
                if (arg == 0)
                {
                    return Class1.GetMoreStatic(arg);
                }
            }
            catch(Exception ex)
            {
                Class1.GetMoreStatic(arg);
                return 0;
            }

            return arg - 1;
        }

        public static int TryCatchFinallySimple(int arg)
        {
            arg = Class1.GetMoreStatic(arg);

            try
            {
                arg = Class1.GetMoreStatic(arg);
            }
            catch (Exception)
            {
                Class1.GetMoreStatic(arg);
            }
            finally
            {
                arg -= 2;
            }

            return arg - 10;
        }
    }
}
