using System.Diagnostics;

using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

using T4CodeGenerator.Generators.Core;

namespace T4CodeGenerator.ReSharperExtension
{
    public class GeneratorDataProvider : IGeneratorDataProvider
    {
        private ICSharpControlFlowAnalysisResult _cachedControlFlowGraphInspectionResult;
        private bool _controlFlowGraphInspected;
        private ICSharpControlFlowGraph _cachedControlFlowGraph;
        private bool _controlFlowGraphCached;

        public ISolution Solution { get; }

        public IProject Project { get; }

        public IPsiModule PsiModule { get; }

        public IPsiServices PsiServices { get; }

        public IPsiSourceFile SourceFile { get; }

        public ICSharpFile PsiFile { get; }

        public ITreeNode SelectedElement { get; }

        public IDocument Document { get; }

        public TreeTextRange SelectedTreeRange { get; }

        public DocumentRange DocumentSelection { get; }

        public ITreeNode TokenAfterCaret { get; }

        public ITreeNode TokenBeforeCaret { get; }

        public CSharpElementFactory ElementFactory { get; }

        public GeneratorDataProvider(ICSharpContextActionDataProvider cSharpContextActionDataProvider)
        {
            this.Solution = cSharpContextActionDataProvider.Solution;
            this.Project = cSharpContextActionDataProvider.Project;
            this.PsiModule = cSharpContextActionDataProvider.PsiModule;
            this.PsiServices = cSharpContextActionDataProvider.PsiServices;
            this.SourceFile = cSharpContextActionDataProvider.SourceFile;
            this.PsiFile = cSharpContextActionDataProvider.PsiFile;
            this.SelectedElement = cSharpContextActionDataProvider.SelectedElement;
            this.Document = cSharpContextActionDataProvider.Document;
            this.SelectedTreeRange = cSharpContextActionDataProvider.SelectedTreeRange;
            this.DocumentSelection = cSharpContextActionDataProvider.DocumentSelection;
            this.TokenAfterCaret = cSharpContextActionDataProvider.TokenAfterCaret;
            this.TokenBeforeCaret = cSharpContextActionDataProvider.TokenBeforeCaret;
            this.ElementFactory = cSharpContextActionDataProvider.ElementFactory;
        }

        private GeneratorDataProvider(
            ISolution solution,
            IProject project,
            IPsiModule psiModule,
            IPsiServices psiServices,
            IPsiSourceFile sourceFile,
            ICSharpFile psiFile,
            ITreeNode selectedElement,
            IDocument document,
            TreeTextRange selectedTreeRange,
            DocumentRange documentSelection,
            ITreeNode tokenAfterCaret,
            ITreeNode tokenBeforeCaret,
            CSharpElementFactory elementFactory)
        {
            this.Solution = solution;
            this.Project = project;
            this.PsiModule = psiModule;
            this.PsiServices = psiServices;
            this.SourceFile = sourceFile;
            this.PsiFile = psiFile;
            this.SelectedElement = selectedElement;
            this.Document = document;
            this.SelectedTreeRange = selectedTreeRange;
            this.DocumentSelection = documentSelection;
            this.TokenAfterCaret = tokenAfterCaret;
            this.TokenBeforeCaret = tokenBeforeCaret;
            this.ElementFactory = elementFactory;
        }

        public IGeneratorDataProvider CloneWithReplacedSelectedElement(ITreeNode selectedElement)
        {
            IPsiSourceFile sourceFile = selectedElement.GetSourceFile();

            Debug.Assert(sourceFile != null, "psiSourceFile != null");

            var psiFile = (ICSharpFile)sourceFile.GetPsiFile<CSharpLanguage>(selectedElement.GetDocumentRange());
            TreeOffset treeStartOffset = selectedElement.GetTreeStartOffset();

            Debug.Assert(psiFile != null, "psiFile != null");

            return new GeneratorDataProvider(
                this.Solution,
                selectedElement.GetProject(),
                this.PsiModule,
                this.PsiServices,
                sourceFile,
                psiFile,
                selectedElement,
                sourceFile.Document,
                selectedElement.GetTreeTextRange(),
                selectedElement.GetDocumentRange(),
                psiFile.FindTokenAt(treeStartOffset),
                treeStartOffset.Offset > 0 ? psiFile.FindNodeAt(treeStartOffset - 1) : null,
                this.ElementFactory
            );
        }

        public ICSharpControlFlowGraph GetControlFlowGraph()
        {
            if (!_controlFlowGraphCached)
            {
                if (this.SelectedElement is ICSharpTreeNode)
                {
                    ITreeNode containingGraphOwner = ControlFlowBuilder.GetContainingGraphOwner(this.SelectedElement);
                    if (containingGraphOwner != null)
                    {
                        new CachingNonQualifiedReferencesResolver().Process(containingGraphOwner);
                        _cachedControlFlowGraph = ControlFlowBuilder.GetGraph(containingGraphOwner) as ICSharpControlFlowGraph;
                    }
                }

                _controlFlowGraphCached = true;
            }

            return _cachedControlFlowGraph;
        }

        public ICSharpControlFlowAnalysisResult InspectControlFlowGraph()
        {
            if (!_controlFlowGraphInspected)
            {
                ICSharpControlFlowGraph controlFlowGraph = this.GetControlFlowGraph();
                if (controlFlowGraph != null)
                {
                    ValueAnalysisMode analysisMode = this.SelectedElement.NotNull("context != null").GetSettingsStore().GetValue(HighlightingSettingsAccessor.ValueAnalysisMode);
                    CSharpControlFlowGraphInspector flowGraphInspector = CSharpControlFlowGraphInspector.Inspect(controlFlowGraph, analysisMode);
                    if (!flowGraphInspector.HasComplexityOverflow)
                    {
                        _cachedControlFlowGraphInspectionResult = flowGraphInspector;
                    }
                }

                _controlFlowGraphInspected = true;
            }

            return _cachedControlFlowGraphInspectionResult;
        }
    }
}