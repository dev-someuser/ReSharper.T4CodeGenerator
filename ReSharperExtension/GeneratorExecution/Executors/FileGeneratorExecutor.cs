using System;
using System.Linq;

using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.Application.UI.PopupLayout;
using JetBrains.DocumentManagers;
using JetBrains.DocumentManagers.impl;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;

using T4CodeGenerator.Generators.Core.Contracts;
using T4CodeGenerator.Generators.Core.Helpers;
using T4CodeGenerator.ReSharperExtension.GeneratorExecution.Helpers;

namespace T4CodeGenerator.ReSharperExtension.GeneratorExecution.Executors
{
    [ShellFeaturePart]
    public class FileGeneratorExecutor : GeneratorExecutorBase<IFileGenerator>
    {
        protected override bool TryExecute(IFileGenerator generator, GeneratorExecutionHost executionHost)
        {
            ISolution solution = generator.DataProvider.Solution;
            IProjectFile sourceFile = generator.DataProvider.SourceFile.ToProjectFile();

            if (!ExecutionHelpers.TryExecuteGenerator(generator, out string generatedText, out string filePath))
            {
                return false;
            }

            bool fileCreated = TryGenerateFile(
                solution, 
                sourceFile, 
                filePath, 
                generatedText, 
                out IProjectFile generatedFile
            );

            if (!fileCreated)
            {
                return false;
            }

            NavigateToFile(solution, generatedFile);

            return true;
        }

        private static bool TryGenerateFile(
            ISolution solution, 
            IProjectFile sourceFile, 
            string filePath, 
            string generatedText,
            out IProjectFile generatedFile)
        {
            using (var progressIndicator = NullProgressIndicator.Create())
            {
                IProjectModelTransactionCookie transaction =
                    solution.CreateTransactionCookie(DefaultAction.Rollback, "T4 Generating file", progressIndicator);
                using (transaction)
                {
                    if (!TryCreateFile(filePath, solution, sourceFile, transaction, out generatedFile))
                    {
                        return false;
                    }

                    IDocument generatedDocument = generatedFile.GetDocument();
                    generatedDocument.ReplaceText(generatedDocument.DocumentRange, generatedText);

                    ExecutionHelpers.OrginizeUsingsAndFormatFile(solution, generatedDocument);

                    transaction.Commit(progressIndicator);
                }
            }
            
            return true;
        }

        public static bool TryCreateFile(
            string filePath,
            ISolution solution,
            IProjectFile sourceFile,
            IProjectModelTransactionCookie transaction,
            out IProjectFile file)
        {
            file = null;

            string failReason;

            if (!SolutionFilePathHelpers.TryParseFilePath(filePath, out RelativePath relativeFilePath, out failReason))
            {
                MessageBox.ShowError(failReason);
                return false;
            }

            if (!SolutionFilePathHelpers.TryGetProject(solution, relativeFilePath, out IProject project, out failReason))
            {
                MessageBox.ShowError(failReason);
                return false;
            }

            if (!TryGetOrCreateProjectFolder(relativeFilePath, project, transaction, out IProjectFolder projectFolder))
            {
                return false;
            }

            return TryEnsureFileExists(relativeFilePath, projectFolder, sourceFile, transaction, out file);
        }

        private static bool TryGetOrCreateProjectFolder(
            RelativePath relativeFilePath,
            IProject project,
            IProjectModelTransactionCookie transaction,
            out IProjectFolder projectFolder)
        {
            RelativePath subFolderRelativePath =
                SolutionFilePathHelpers.GetProjectSubFolderRelativePath(relativeFilePath, project);

            if (!SolutionFilePathHelpers.TryGetProjectFolder(subFolderRelativePath, project, out projectFolder))
            {
                string message = 
                    $"There is not folder '{subFolderRelativePath}' in project '{project.Name}'.{Environment.NewLine}" +
                    "Do you want to create it?";

                if (!MessageBox.ShowYesNo(message))
                {
                    return false;
                }

                FileSystemPath fileSystemPath = subFolderRelativePath.MakeAbsoluteBasedOn(project.Location);
                projectFolder = project.GetOrCreateProjectFolder(fileSystemPath, transaction);
            }

            return true;
        }

        private static bool TryEnsureFileExists(
            RelativePath relativeFilePath,
            IProjectFolder projectFolder,
            IProjectFile sourceFile,
            IProjectModelTransactionCookie transaction,
            out IProjectFile file)
        {
            file = projectFolder.GetSubFiles(relativeFilePath.Name).FirstOrDefault();
            if (file != null)
            {
                var message = 
                    $"The file '{relativeFilePath.FullPath}' already exists.{Environment.NewLine}" +
                    "Do you want to overwrite it?";

                return MessageBox.ShowYesNo(message);
            }

            file = ExecutionHelpers.CreateFile(relativeFilePath.Name, projectFolder, sourceFile, transaction);

            return true;
        }

        private static void NavigateToFile(ISolution solution, IProjectFile generatedFile)
        {
            var navigationManager = NavigationManager.GetInstance(solution);
            var navigationPoint = new ProjectFileNavigationPoint(generatedFile);
            var navigationOptions = NavigationOptions.FromWindowContext(
                Shell.Instance.GetComponent<IMainWindowPopupWindowContext>().Source, 
                "", 
                false /* activate */
            );

            navigationManager.Navigate(navigationPoint, navigationOptions);
        }
    }
}