using System;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.Util;

using T4CodeGenerator.Generators.Core.StructuralSearch;

namespace T4CodeGenerator.Generators.Core.Generators.Common
{
    public abstract class ByPatternInPlaceGeneratorBase : InPlaceGeneratorBase
    {
        protected IStructuralMatchResult MatchResult { get; private set; }

        protected abstract ByPatternInPlaceGenerationMode GenerationMode { get; }

        protected abstract string Pattern { get; }

        public sealed override TextRange GetTextRangeToDelete()
        {
            if (this.GenerationMode == ByPatternInPlaceGenerationMode.ReplaceMatchedText)
            {
                return this.MatchResult.GetDocumentRange().TextRange;
            }
            
            return TextRange.FromLength(0);
        }

        public sealed override int GetPositionToInsert()
        {
            switch (this.GenerationMode)
            {
                case ByPatternInPlaceGenerationMode.ReplaceMatchedText:
                case ByPatternInPlaceGenerationMode.InsertBeforeMatchedText:
                    return this.MatchResult.GetDocumentRange().StartOffset.Offset;
                case ByPatternInPlaceGenerationMode.InsertAfterMatchedText:
                    return this.MatchResult.GetDocumentRange().EndOffset.Offset;
                case ByPatternInPlaceGenerationMode.InsertAfterCarret:
                    return this.DataProvider.DocumentSelection.StartOffset.Offset;
                case ByPatternInPlaceGenerationMode.InsertInCustomPostion:
                    return GetCustomPositionToInsert();
                default:
                    throw new InvalidOperationException($"The value {nameof(ByPatternInPlaceGenerationMode)}.{this.GenerationMode} was not expected.");
            }
        }

        public sealed override bool IsAvailable()
        {
            StructuralSearchEngine structuralSearchEngine =
                this.DataProvider.Solution.GetComponent<StructuralSearchEngine>();

            IStructuralMatcher structuralMatcher = structuralSearchEngine.CreateMatcherWithPlaceholdersBindingObject(
                this.Pattern,
                CSharpLanguage.Instance,
                this.GetType(),
                GetAdditionalPlaceholders()
            );

            if (structuralMatcher != null)
            {
                IStructuralMatchResult matchResult =
                    structuralMatcher.IsTreeNodeOrParentsMatched(this.DataProvider.SelectedElement, this);

                if (matchResult.Matched)
                {
                    this.MatchResult = matchResult;

                    return IsAvailableCore();
                }
            }

            return false;
        }

        protected virtual int GetCustomPositionToInsert() => throw new NotImplementedException();

        protected virtual bool IsAvailableCore() => true;

        protected virtual IPlaceholder[] GetAdditionalPlaceholders() => null;
    }
}