// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc.RazorPages.Razevolution;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure
{
    public class PageActionDescriptorChangeProvider : IActionDescriptorChangeProvider
    {
        private readonly RazorProject _project;

        public PageActionDescriptorChangeProvider(RazorProject project)
        {
            _project = project;
        }

        public IChangeToken GetChangeToken() => _project.GetChangeToken("/Pages/**/*.razor");
    }
}
