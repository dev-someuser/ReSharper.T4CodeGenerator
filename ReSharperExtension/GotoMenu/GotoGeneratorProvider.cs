using System;
using System.Collections.Generic;
using System.Diagnostics;

using JetBrains.Application;
using JetBrains.Application.UI.Utils;
using JetBrains.ReSharper.Feature.Services.Navigation.Goto.Misc;
using JetBrains.ReSharper.Feature.Services.Navigation.Goto.ProvidersAPI;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.Text;
using JetBrains.Util;

using T4CodeGenerator.Generators.Core.Contracts;
using T4CodeGenerator.ReSharperExtension.GeneratorsSource;

namespace T4CodeGenerator.ReSharperExtension.GotoMenu
{
    [ShellFeaturePart]
    public class GotoGeneratorProvider : IGotoGeneratorProvider
    {
        public bool IsApplicable(INavigationScope scope, GotoContext gotoContext, IIdentifierMatcher matcher)
        {
            return scope is SolutionNavigationScope;
        }

        public IEnumerable<Pair<IOccurrence, MatchingInfo>> GetMatchingOccurrences(
            IIdentifierMatcher matcher, 
            INavigationScope scope, 
            GotoContext gotoContext,
            Func<bool> checkForInterrupt)
        {
            ApplicableGeneratorsCache applicableGeneratorsCache =
                gotoContext.GetData(GotoGeneratorController.ApplicableGeneratorsCacheKey);

            Debug.Assert(applicableGeneratorsCache != null, "applicableGeneratorsCache != null");

            foreach (IGenerator generator in applicableGeneratorsCache.GetApplicableGenerators())
            {
                string name = generator.Name;
                string typeName = generator.GetType().Name;

                bool matches = matcher.Matches(name) || matcher.Matches(typeName);

                if (matches)
                {
                    yield return Pair.Of(
                        (IOccurrence)new GeneratorOccurrence(generator),
                        new MatchingInfo(matcher, typeName)
                    );
                }
            }
        }

        public IEnumerable<MatchingInfo> FindMatchingInfos(
            IIdentifierMatcher matcher, 
            INavigationScope scope, 
            GotoContext gotoContext,
            Func<bool> checkForInterrupt)
        {
            yield break;
        }

        public IEnumerable<IOccurrence> GetOccurrencesByMatchingInfo(
            MatchingInfo navigationInfo, 
            INavigationScope scope,
            GotoContext gotoContext,
            Func<bool> checkForInterrupt)
        {
            yield break;
        }
    }
}