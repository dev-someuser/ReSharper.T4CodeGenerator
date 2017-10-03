using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;

namespace T4CodeGenerator.Generators.Core.StructuralSearch.Attributes
{
    public class IdentifierPlaceholderAttribute : PlaceholderAttributeBase
    {
        public string NameRegex { get; set; }

        public string Type { get; set; }

        public bool ExactType { get; set; }

        public bool NameRegexIsCaseSensitive { get; set; }

        public override IPlaceholder CreatePlaceholder(string name)
        {
            return new IdentifierPlaceholder(
                name,
                this.NameRegex,
                this.NameRegexIsCaseSensitive,
                this.Type,
                this.ExactType
            );
        }
    }
}