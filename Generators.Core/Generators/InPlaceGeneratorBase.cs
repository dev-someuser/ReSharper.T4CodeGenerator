using System.Collections.Generic;

using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.Util;

using T4CodeGenerator.Generators.Core.Contracts;

namespace T4CodeGenerator.Generators.Core.Generators
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public abstract class InPlaceGeneratorBase : GeneratorBase, IInPlaceGenerator
    {
        private readonly HashSet<IUsingDirective> _missingUsingDirectives = new HashSet<IUsingDirective>();

        public IReadOnlyCollection<IUsingDirective> MissingUsingDirectives => _missingUsingDirectives;

        public virtual TextRange GetTextRangeToDelete() => new TextRange(0);

        public abstract int GetPositionToInsert();

        protected void ImportNamespace(string @namespace)
        {
            ProcessImportedNamespace(@namespace);
        }

        protected override void ProcessImportedNamespace(string @namespace)
        {
            IUsingDirective usingDirective = 
                this.DataProvider.ElementFactory.CreateUsingDirective($"using {@namespace}");
            bool alreadyImported = 
                this.DataProvider.PsiFile.Imports.Any(x => x.ImportedSymbolName.QualifiedName == usingDirective.ImportedSymbolName.QualifiedName);
            if (!alreadyImported)
            {
                _missingUsingDirectives.Add(usingDirective);
            }
        }
    }
}