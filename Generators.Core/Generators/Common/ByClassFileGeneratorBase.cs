using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace T4CodeGenerator.Generators.Core.Generators.Common
{
    public abstract class ByClassFileGeneratorBase : FileGeneratorBase
    {
        protected IClassLikeDeclaration Class { get; private set; }

        public sealed override bool IsAvailable()
        {
            this.Class = this.DataProvider.SelectedElement.Parent as IClassLikeDeclaration;

            return
                this.Class != null &&
                this.DataProvider.SelectedElement == this.Class.NameIdentifier &&
                IsAvailableCore();
        }

        public virtual bool IsAvailableCore() => true;
    }
}