﻿using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;

namespace T4CodeGenerator.Generators.Core.StructuralSearch.Attributes
{
    public class ArgumentPlaceholderAttribute : PlaceholderAttributeBase
    {
        public int MinimalOccurrences { get; set; } = -1;

        public int MaximalOccurrences { get; set; } = -1;

        public override IPlaceholder CreatePlaceholder(string name)
        {
            return new ArgumentPlaceholder(name, this.MinimalOccurrences, this.MaximalOccurrences);
        }
    }
}