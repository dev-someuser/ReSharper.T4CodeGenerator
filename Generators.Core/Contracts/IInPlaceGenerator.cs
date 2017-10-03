using System.Collections.Generic;

using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.Util;

namespace T4CodeGenerator.Generators.Core.Contracts
{
    public interface IInPlaceGenerator : IGenerator
    {
        IReadOnlyCollection<IUsingDirective> MissingUsingDirectives { get; }

        TextRange GetTextRangeToDelete();

        int GetPositionToInsert();

        string TransformText();
    }
}