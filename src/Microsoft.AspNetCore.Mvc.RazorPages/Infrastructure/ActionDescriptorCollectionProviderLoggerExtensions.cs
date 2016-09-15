// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure
{
    public static class ActionDescriptorCollectionProviderLoggerExtensions
    {
        private static readonly double TimestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;
        private static readonly Action<ILogger, Exception> _createActionDescriptorsStart;
        private static readonly Action<ILogger, double, Exception> _createActionDescriptorsEnd;

        static ActionDescriptorCollectionProviderLoggerExtensions()
        {
            _createActionDescriptorsStart = LoggerMessage.Define(
               LogLevel.Debug,
               1,
               "Creating ActionDescriptorCollection started.");

            _createActionDescriptorsEnd = LoggerMessage.Define<double>(
                LogLevel.Debug,
                2,
                "Creating ActionDescriptorCollection completed in {ElapsedMilliseconds}ms.");
        }

        public static void CreateActionDescriptorCollectionStart(this ILogger logger)
        {
            _createActionDescriptorsStart(logger, null);
        }

        public static void CreateActionDescriptorCollectionEnd(this ILogger logger, long startTimestamp)
        {
            // Don't log if logging wasn't enabled at start of request as time will be wildly wrong.
            if (startTimestamp != 0)
            {
                var currentTimestamp = Stopwatch.GetTimestamp();
                var elapsed = new TimeSpan((long)(TimestampToTicks * (currentTimestamp - startTimestamp)));
                _createActionDescriptorsEnd(logger, elapsed.TotalMilliseconds, null);
            }
        }
    }
}
