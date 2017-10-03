using System;
using System.Collections.Generic;
using System.Diagnostics;

using JetBrains.Annotations;
using JetBrains.Application.DataContext;
using JetBrains.Application.Resources;
using JetBrains.Application.Threading;
using JetBrains.Application.UI.Controls.GotoByName;
using JetBrains.Application.UI.Controls.JetPopupMenu.Detail;
using JetBrains.Application.UI.PopupLayout;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Feature.Services.Navigation.Goto.Controllers;
using JetBrains.ReSharper.Feature.Services.Navigation.Goto.Filters;
using JetBrains.ReSharper.Feature.Services.Navigation.Goto.Misc;
using JetBrains.ReSharper.Feature.Services.Navigation.Goto.ProvidersAPI;
using JetBrains.ReSharper.Feature.Services.Navigation.Goto.ProvidersAPI.ChainedProviders;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.TextControl;
using JetBrains.TextControl.DataContext;
using JetBrains.Util;

using T4CodeGenerator.Generators.Core;
using T4CodeGenerator.ReSharperExtension.GeneratorExecution;
using T4CodeGenerator.ReSharperExtension.GeneratorsSource;

namespace T4CodeGenerator.ReSharperExtension.GotoMenu
{
    public class GotoGeneratorController : GotoControllerBase<
        IGotoGeneratorProvider,
        IChainedSearchProvider,
        IInstantGotoGeneratorProvider>
    {
        public static readonly Key<ApplicableGeneratorsCache> ApplicableGeneratorsCacheKey = 
            new Key<ApplicableGeneratorsCache>("ApplicableGeneratorsCacheKey");

        private readonly GeneratorExecutionHost _executionHost;

        protected override bool InstantItemsAllowed => true;

        protected override bool ShouldScoreDefaultResults => true;

        protected override int InstantItemsLimit => 20;

        public GotoGeneratorController(
            [NotNull] Lifetime lifetime,
            [NotNull] ISolution solution,
            [NotNull] IShellLocks locks,
            [NotNull] IDataContext context,
            IMainWindowPopupWindowContext mainWindowPopupWindowContext,
            bool enableMulticore) : 
            base(lifetime, solution, solution, LibrariesFlag.SolutionOnly, locks, enableMulticore, new GotoByNameModel(lifetime), mainWindowPopupWindowContext)
        {
            this.EtcItemIcon = IdeThemedIcons.SearchResults.Id;
            this.ItemsPassFilter = new Property<Func<IOccurrence, bool>>("ItemsPassFilter", this.DefaultFilter);
            GotoByNameModelManager instance = GotoByNameModelManager.GetInstance(solution);
            instance.ProcessModel<GotoGeneratorModelInitilizer>(this.Model, lifetime, this.GetType());

            ITextControl textControl = context.GetData(TextControlDataConstants.TEXT_CONTROL);
            Debug.Assert(textControl != null, "textControl != null");

            var dataBuilder = new CSharpContextActionDataBuilder();
            var generatorDataProvider =  new GeneratorDataProvider((ICSharpContextActionDataProvider)dataBuilder.Build(solution, textControl));

            var generatorsProvider = solution.GetComponent<GeneratorsProvider>();
            var applicableGeneratorsCache = new ApplicableGeneratorsCache(generatorsProvider, generatorDataProvider);

            this.GotoContext.PutData(ApplicableGeneratorsCacheKey, applicableGeneratorsCache);

            _executionHost = context.GetComponent<GeneratorExecutionHost>();
        }

        protected override ICollection<ChainedNavigationItemData> InitScopes(
            bool isSearchingInLibs,
            INavigationProviderFilter navfilter)
        {
            return new[]
            {
                new ChainedNavigationItemData(
                    null /* firstWordMatchingInfo */, 
                    new SolutionNavigationScope(this.ScopeData as ISolution, false, navfilter)
                )
            };
        }

        protected override OccurrencePresentationOptions GetPresentationOptions()
        {
            if (this.WordsCount > 1)
            {
                return new OccurrencePresentationOptions
                {
                    TextDisplayStyle = TextDisplayStyle.ChainedPME
                };
            }

            return new OccurrencePresentationOptions
            {
                TextDisplayStyle = TextDisplayStyle.ContainingFile
            };
        }

        protected override bool ExecuteItem(JetPopupMenuItem item, ISignal<bool> closeBeforeExecute)
        {
            if (item.Key is GeneratorOccurrence occurance)
            {
                return _executionHost.TryExecute(occurance.Generator);
            }

            return false;
        }

        public bool DefaultFilter(IOccurrence occurrence)
        {
            return true;
        }

        public override bool IsSpecialString(ref string filterString)
        {
            return false;
        }
    }
}