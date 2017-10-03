using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace T4CodeGenerator.Generators.Core.Generators.Common
{
    public abstract class ByPropertyInPlaceGeneratorBase : InPlaceGeneratorBase
    {
        public IPropertyDeclaration Property { get; set; }

        public sealed override bool IsAvailable()
        {
            this.Property = this.DataProvider.SelectedElement?.Parent as IPropertyDeclaration;

            return
                this.Property != null &&
                this.DataProvider.SelectedElement == this.Property?.NameIdentifier &&
                IsAvailableCore();
        }

        public virtual bool IsAvailableCore() => true;
    }
}