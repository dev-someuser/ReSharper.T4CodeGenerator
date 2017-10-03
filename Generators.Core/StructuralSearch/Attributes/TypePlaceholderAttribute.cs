using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;

namespace T4CodeGenerator.Generators.Core.StructuralSearch.Attributes
{
    public class TypePlaceholderAttribute : PlaceholderAttributeBase
    {
        public string Type { get; set; }

        public bool ExactType { get; set; }

        public override IPlaceholder CreatePlaceholder(string name)
        {
            return new TypePlaceholder(name, this.Type, this.ExactType);
        }
    }
}