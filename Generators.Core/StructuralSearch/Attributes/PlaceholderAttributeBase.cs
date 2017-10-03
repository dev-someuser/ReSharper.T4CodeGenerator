using System;

using JetBrains.ReSharper.Feature.Services.StructuralSearch;

namespace T4CodeGenerator.Generators.Core.StructuralSearch.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class PlaceholderAttributeBase : Attribute
    {
        public abstract IPlaceholder CreatePlaceholder(string name);
    }
}