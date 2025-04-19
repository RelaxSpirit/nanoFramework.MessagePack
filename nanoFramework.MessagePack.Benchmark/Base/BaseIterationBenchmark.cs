﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System;

namespace nanoFramework.MessagePack.Benchmark.Base
{
    public abstract class BaseIterationBenchmark
    {
        protected virtual int IterationCount => 20;

        public void RunInIteration(Action methodToRun)
        {
            if (methodToRun == null)
            {
                throw new ArgumentNullException(nameof(methodToRun));
            }
            else
            {
                int step = 0;
                while (step++ < IterationCount)
                {
                    methodToRun();
                }
            }
        }
    }
}
