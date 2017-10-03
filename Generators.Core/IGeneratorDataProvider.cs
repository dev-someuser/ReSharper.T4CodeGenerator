using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;

namespace T4CodeGenerator.Generators.Core
{
    public interface IGeneratorDataProvider
    {
        ISolution Solution { get; }

        IProject Project { get; }

        IPsiModule PsiModule { get; }

        IPsiServices PsiServices { get; }

        IPsiSourceFile SourceFile { get; }

        ICSharpFile PsiFile { get; }

        ITreeNode SelectedElement { get; }

        IDocument Document { get; }

        TreeTextRange SelectedTreeRange { get; }

        DocumentRange DocumentSelection { get; }

        ITreeNode TokenAfterCaret { get; }

        ITreeNode TokenBeforeCaret { get; }

        CSharpElementFactory ElementFactory { get; }

        ICSharpControlFlowGraph GetControlFlowGraph();

        ICSharpControlFlowAnalysisResult InspectControlFlowGraph();

        IGeneratorDataProvider CloneWithReplacedSelectedElement(ITreeNode selectedElement);
    }
}