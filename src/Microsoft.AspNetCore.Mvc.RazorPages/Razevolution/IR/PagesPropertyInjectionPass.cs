// Copyright(c) .NET Foundation.All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Mvc.RazorPages.Razevolution.IR
{
    // TODO: Should we encapsulate injected properties as inject chunks?
    public class PagesPropertyInjectionPass : ICSharpSourceTreePass
    {
        public RazorEngine Engine { get; set; }

        public int Order { get; } = -1;

        public CSharpSourceTree Execute(RazorCodeDocument document, CSharpSourceTree sourceTree)
        {
            string modelType = null;

            // Directives are always harvested at the top of the document, no need to recurse deeper.
            for (var i = 0; i < sourceTree.Children.Count; i++)
            {
                var directive = sourceTree.Children[i] as RazorDirective;
                if (string.Equals(directive?.Name, "model", StringComparison.Ordinal))
                {
                    modelType = directive.Data["TypeName"];
                    break;
                }
            }

            var classInfo = document.GetClassName();
            if (modelType == null)
            {
                // Insert a model directive into the system so sub-systems can rely on the model being the class.
                var modelDirective = new RazorDirective
                {
                    Name = "model",
                    Data = new Dictionary<string, string>
                    {
                        ["TypeName"] = classInfo.Class,
                    },
                };
                sourceTree.Children.Insert(0, modelDirective);
                modelType = classInfo.Class;
            }

            // Inject properties needed to execute Razor pages.
            var classDeclaration = FindClassDeclaration(sourceTree);

            var viewDataType = $"global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<{modelType}>";
            classDeclaration.Children.Insert(0, new RenderStatement()
            {
                Code = $"public {modelType} Model => ViewData.Model;",
            });

            var viewDataProperty = new RenderStatement
            {
                Code = $"public new {viewDataType} ViewData => ({viewDataType})base.ViewData;"
            };
            classDeclaration.Children.Insert(0, viewDataProperty);

            var injectHtmlHelper = new RazorDirective
            {
                Name = "inject",
                Data = new Dictionary<string, string>
                {
                    ["TypeName"] = $"Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<{modelType}>",
                    ["MemberName"] = "Html"
                },
            };
            classDeclaration.Children.Insert(0, injectHtmlHelper);

            var injectLogger = new RazorDirective
            {
                Name = "inject",
                Data = new Dictionary<string, string>
                {
                    ["TypeName"] = $"Microsoft.Extensions.Logging.ILogger<{classInfo.Class}>",
                    ["MemberName"] = "Logger"
                },
            };
            classDeclaration.Children.Insert(0, injectLogger);

            return sourceTree;
        }

        private static ViewClassDeclaration FindClassDeclaration(CSharpBlock block)
        {
            if (block is ViewClassDeclaration)
            {
                return (ViewClassDeclaration)block;
            }

            for (var i = 0; i < block.Children.Count; i++)
            {
                var currentBlock = block.Children[i] as CSharpBlock;
                if (currentBlock == null)
                {
                    continue;
                }

                var classDeclaration = FindClassDeclaration(currentBlock);
                if (classDeclaration != null)
                {
                    return classDeclaration;
                }
            }

            return null;
        }
    }
}
