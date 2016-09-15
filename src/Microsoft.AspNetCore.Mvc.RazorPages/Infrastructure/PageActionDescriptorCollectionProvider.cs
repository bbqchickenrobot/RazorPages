using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure
{
    public class PageActionDescriptorCollectionProvider : IActionDescriptorCollectionProvider
    {
        private readonly object _collectionLock = new object();
        private readonly IActionDescriptorProvider[] _providers;
        private int _descriptorVersion;
        private ActionDescriptorCollection _collection;

        public PageActionDescriptorCollectionProvider(
            IEnumerable<IActionDescriptorProvider> actionDescriptors)
        {
            _providers = actionDescriptors.OrderBy(p => p.Order).ToArray();
            foreach (var provider in actionDescriptors.OfType<IMutableActionDescriptorProvider>())
            {
                ChangeToken.OnChange(provider.GetChangeToken, OnChange);
            }
        }

        public ActionDescriptorCollection ActionDescriptors
        {
            get
            {
                lock (_collectionLock)
                {
                    if (_collection == null)
                    {
                        _collection = GetCollection();
                    }

                    return _collection;
                }
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
            lock (_collectionLock)
            {
                _collection = null;
            }
        }
    }
}
