using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;

namespace T4CodeGenerator.Generators.Core.StructuralSearch.Attributes
{
    public class ExpressionPlaceholderAttribute : PlaceholderAttributeBase
    {
        public string ExpressionType { get; set; }

        public bool ExactType { get; set; }

        public override IPlaceholder CreatePlaceholder(string name)
        {
            return new ExpressionPlaceholder(name, this.ExpressionType, this.ExactType);
        }
    }
}