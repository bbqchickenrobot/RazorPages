// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc.RazorPages.Razevolution.IR;

namespace Microsoft.AspNetCore.Mvc.RazorPages.Razevolution
{
    public interface ICSharpSourceLoweringFeature : IRazorEngineFeature
    {
        CSharpSourceTree Execute(RazorCodeDocument document, RazorChunkTree chunkTree);
    }
}