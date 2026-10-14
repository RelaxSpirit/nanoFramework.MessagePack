// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace nanoFramework.MessagePack.Benchmark.Base
{
    /// <summary>
    /// Base benchmark class.
    /// </summary>
    public abstract class BaseIterationBenchmark
    {
        private const int VirtualDeviceIterationCount = 20;
        private const int PhysicalDeviceIterationCount = 1;
        private const string VirtualDeviceName = "Virtual nanoDevice";

        private static readonly int DefaultIterationCount;

        static BaseIterationBenchmark()
        {
             DefaultIterationCount = VirtualDeviceName == Runtime.Native.SystemInfo.TargetName ? VirtualDeviceIterationCount : PhysicalDeviceIterationCount;
        }

        /// <summary>
        /// Call iteration benchmark method.
        /// </summary>
        /// <param name="methodToRun">Iteration benchmark method.</param>
        /// <exception cref="ArgumentNullException">iteration benchmark method is <see  cref="null"/></exception>
        protected void RunInIteration(Action methodToRun)
        {
            if (methodToRun == null)
            {
                throw new ArgumentNullException(nameof(methodToRun));
            }
            else
            {
                int step = 0;
                while (step++ < DefaultIterationCount)
                {
                    methodToRun();
                }
            }
        }
    }
}
