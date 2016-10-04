// Copyright(c) .NET Foundation.All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages.Razevolution.Directives;

namespace Microsoft.AspNetCore.Mvc.RazorPages.Razevolution.IR
{
    public interface IRazorDirective : ICSharpSource
    {
        string Name { get; }

        IList<RazorDirectiveToken> Tokens { get; }
    }
}
