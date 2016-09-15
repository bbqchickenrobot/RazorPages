using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Mvc.Abstractions
{
    public interface IMutableActionDescriptorProvider : IActionDescriptorProvider
    {
        IChangeToken GetChangeToken();
    }
}
