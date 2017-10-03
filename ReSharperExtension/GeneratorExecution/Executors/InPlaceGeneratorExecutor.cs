using System.Collections.Generic;

using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.Util;

using T4CodeGenerator.Generators.Core.Contracts;
using T4CodeGenerator.ReSharperExtension.GeneratorExecution.Helpers;

namespace T4CodeGenerator.ReSharperExtension.GeneratorExecution.Executors
{
    [ShellFeaturePart]
    public class InPlaceGeneratorExecutor : GeneratorExecutorBase<IInPlaceGenerator>
    {
        protected override bool TryExecute(IInPlaceGenerator generator, GeneratorExecutionHost executionHost)
        {
            IDocument document = generator.DataProvider.Document;

            bool executed = ExecutionHelpers.TryExecuteGenerator(
                generator, 
                out string generatedText,
                out TextRange rangeToDelete, 
                out int positionToInsert,
                out IReadOnlyCollection<IUsingDirective> missingUsingDirectives
            );

            if (!executed)
            {
                return false;
            }

            ISolution solution = generator.DataProvider.Solution;

            using (var progressIndicator = NullProgressIndicator.Create())
            {
                IProjectModelTransactionCookie transaction =
                    solution.CreateTransactionCookie(DefaultAction.Rollback, "T4 Generating file", progressIndicator);
                using (transaction)
                {
                    if (!rangeToDelete.IsEmpty)
                    {
                        document.DeleteText(rangeToDelete);
                    }

                    document.InsertText(positionToInsert, generatedText);

                    var treeTextRange = TreeTextRange.FromLength(new TreeOffset(positionToInsert), generatedText.Length);
                    ExecutionHelpers.FormatFileRangeAndAddUsingDirectives(solution, document, treeTextRange, missingUsingDirectives);

                    transaction.Commit(progressIndicator);
                }
            }
            
            return true;
        }
    }
}