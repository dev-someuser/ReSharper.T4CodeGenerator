using System.Collections.Generic;

using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

using T4CodeGenerator.Generators.Core.Contracts;

namespace T4CodeGenerator.Generators.Core.Generators
{
    public abstract class MultipleOutputGeneratorBase : IMultipleOutputGenerator
    {
        public IGeneratorDataProvider DataProvider { get; set; }

        public abstract string Name { get; }

        public virtual string Description => null;

        public virtual bool Visible => true;

        public abstract bool IsAvailable();

        public abstract IEnumerable<IGenerator> GetGenerators();

        protected IGenerator CreateChildGenerator<TGenerator>(ITreeNode targetTreeNode)
            where TGenerator: IGenerator, new()
        {
            var generator = new TGenerator();
            generator.DataProvider = this.DataProvider.CloneWithReplacedSelectedElement(targetTreeNode);
            generator.IsAvailable();

            return generator;
        }

        protected ITreeNode GetTypeDeclarationIdentifierNode(IType type)
        {
            ITypeElement typeElement = type.GetTypeElement();
            IClassLikeDeclaration declaration = typeElement?.GetSingleDeclaration<IClassLikeDeclaration>();
            return declaration?.NameIdentifier;
        }
    }
}