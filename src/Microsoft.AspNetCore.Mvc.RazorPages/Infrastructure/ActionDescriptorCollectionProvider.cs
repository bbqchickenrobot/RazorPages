// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure
{
    public class ActionDescriptorCollectionProvider : IActionDescriptorCollectionProvider
    {
        private readonly object _collectionLock = new object();
        private readonly IActionDescriptorProvider[] _providers;
        private readonly ILogger _logger;
        private int _descriptorVersion;
        private ActionDescriptorCollection _collection;

        public ActionDescriptorCollectionProvider(
            IEnumerable<IActionDescriptorProvider> actionDescriptors,
            IEnumerable<IActionDescriptorChangeProvider> changeProviders,
            ILoggerFactory loggerFactory)
        {
            _providers = actionDescriptors.OrderBy(p => p.Order).ToArray();
            _logger = loggerFactory.CreateLogger<ActionDescriptorCollectionProvider>();
            foreach (var provider in changeProviders)
            {
                ChangeToken.OnChange(provider.GetChangeToken, OnChange);
            }
        }

        public ActionDescriptorCollection ActionDescriptors
        {
            get
            {
                if (_collection == null)
                {
                    _logger.CreateActionDescriptorCollectionStart();
                    var startTimestamp = _logger.IsEnabled(LogLevel.Debug) ? Stopwatch.GetTimestamp() : 0;
                    _collection = GetCollection();
                    _logger.CreateActionDescriptorCollectionEnd(startTimestamp);
                }

                return _collection;
            }
        }

        private ActionDescriptorCollection GetCollection()
        {
            var context = new ActionDescriptorProviderContext();

            for (var i = 0; i < _providers.Length; i++)
            {
                _providers[i].OnProvidersExecuting(context);
            }

            for (var i = _providers.Length - 1; i >= 0; i--)
            {
                _providers[i].OnProvidersExecuted(context);
            }

            return new ActionDescriptorCollection(
                new ReadOnlyCollection<ActionDescriptor>(context.Results),
                _descriptorVersion++);
        }

        private void OnChange()
        {
            _collection = null;
        }
    }
}
