using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace T4CodeGenerator.Generators.Core.Generators.Common
{
    public abstract class InsideClassBodyInPlaceGeneratorBase : InPlaceGeneratorBase
    {
        protected IClassLikeDeclaration Class { get; private set; }

        public sealed override bool IsAvailable()
        {
            this.Class = this.DataProvider.SelectedElement.Parent?.Parent as IClassLikeDeclaration;

            return
                this.Class != null &&
                this.DataProvider.SelectedElement.Parent == this.Class.Body &&
                IsAvailableCore();
        }

        public override int GetPositionToInsert() => this.DataProvider.DocumentSelection.StartOffset.Offset;

        protected virtual bool IsAvailableCore() => true;
    }
}