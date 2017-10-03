using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;
using JetBrains.ReSharper.Psi.CSharp;

using T4CodeGenerator.Generators.Core.StructuralSearch;

namespace T4CodeGenerator.Generators.Core.Generators.Common
{
    public abstract class ByPatternFileGeneratorBase : FileGeneratorBase
    {
        protected IStructuralMatchResult MatchResult { get; private set; }

        protected abstract string Pattern { get; }

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

        protected virtual bool IsAvailableCore() => true;

        protected virtual IPlaceholder[] GetAdditionalPlaceholders() => null;
    }
}