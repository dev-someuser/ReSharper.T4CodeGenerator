using System;
using System.Collections.Generic;
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
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;

using T4CodeGenerator.Generators.Core.Contracts;
using T4CodeGenerator.Generators.Core.Helpers;
using T4CodeGenerator.ReSharperExtension.GeneratorExecution.Helpers;

namespace T4CodeGenerator.ReSharperExtension.GeneratorExecution.Executors
{
    [ShellFeaturePart]
    public class MultipleOutputGeneratorExecutor : GeneratorExecutorBase<IMultipleOutputGenerator>
    {
        private class FileGeneratorOutput
        {
            public string FilePath { get; }
            public string GeneratedText { get; }
            public IProjectFile SourceProjectFile { get; }
            public IProject Project { get; set; }
            public RelativePath SubFolderRelativePath { get; set; }
            public string FileName { get; set; }
            public IProjectFolder ProjectFolder { get; set; }
            public IProjectFile GeneratedProjectFile { get; set; }

            public FileGeneratorOutput(string filePath, string generatedText, IProjectFile sourceProjectFile)
            {
                FilePath = filePath;
                GeneratedText = generatedText;
                SourceProjectFile = sourceProjectFile;
            }
        }

        private class InPlaceGeneratorOutput
        {
            public IProjectFile ProjectFile { get; }
            public TextRange RangeToDelete { get; }
            public int PositionToInsert { get; }
            public string GeneratedText { get; }
            public IReadOnlyCollection<IUsingDirective> MissingUsingDirectives { get; }

            public InPlaceGeneratorOutput(
                IProjectFile projectFile,
                TextRange rangeToDelete,
                int positionToInsert,
                string generatedText,
                IReadOnlyCollection<IUsingDirective> missingUsingDirectives)
            {
                ProjectFile = projectFile;
                RangeToDelete = rangeToDelete;
                PositionToInsert = positionToInsert;
                GeneratedText = generatedText;
                MissingUsingDirectives = missingUsingDirectives;
            }
        }

        protected override bool TryExecute(
            IMultipleOutputGenerator generator,
            GeneratorExecutionHost executionHost)
        {
            ISolution solution = generator.DataProvider.Solution;

            IGenerator[] generators = null;
            bool gotGenerators = ExecutionHelpers.TryExecuteGenerator(
                generator,
                () => generators = generator.GetGenerators().ToArray()
            );

            if (!gotGenerators)
            {
                return false;
            }

            bool generatorsExecuted = TryExecuteGenerators(
                generators,
                out List<FileGeneratorOutput> fileGeneratorOutputs,
                out List<InPlaceGeneratorOutput> inPlaceGeneratorOutputs
            );

            if (!generatorsExecuted)
            {
                return false;
            }

            if (!TryGenerateFiles(solution, fileGeneratorOutputs, inPlaceGeneratorOutputs))
            {
                return false;
            }

            NavigateToFiles(solution, fileGeneratorOutputs, inPlaceGeneratorOutputs);

            return true;
        }

        private static bool TryExecuteGenerators(
            IGenerator[] generators,
            out List<FileGeneratorOutput> fileGeneratorOutputs,
            out List<InPlaceGeneratorOutput> inPlaceGeneratorOutputs)
        {
            fileGeneratorOutputs = new List<FileGeneratorOutput>();
            inPlaceGeneratorOutputs = new List<InPlaceGeneratorOutput>();

            List<FileGeneratorOutput> localFileGeneratorOutputs = fileGeneratorOutputs;
            List<InPlaceGeneratorOutput> localInPlaceGeneratorOutputs = inPlaceGeneratorOutputs;

            foreach (IGenerator generator in generators)
            {
                bool executed = ExecutionHelpers.TryExecuteGenerator(
                    generator,
                    () =>
                    {
                        switch (generator)
                        {
                            case IFileGenerator fileGenerator:

                                var fileGeneratorOutput = new FileGeneratorOutput(
                                    fileGenerator.GetFileName(),
                                    fileGenerator.TransformText(),
                                    fileGenerator.DataProvider.SourceFile.ToProjectFile()
                                );

                                localFileGeneratorOutputs.Add(fileGeneratorOutput);
                                break;

                            case IInPlaceGenerator inPlaceGenerator:

                                var inPlaceGeneratorOutput = new InPlaceGeneratorOutput(
                                    inPlaceGenerator.DataProvider.SourceFile.ToProjectFile(),
                                    inPlaceGenerator.GetTextRangeToDelete(),
                                    inPlaceGenerator.GetPositionToInsert(),
                                    inPlaceGenerator.TransformText(),
                                    inPlaceGenerator.MissingUsingDirectives
                                );

                                localInPlaceGeneratorOutputs.Add(inPlaceGeneratorOutput);
                                break;

                            default:
                                throw new NotSupportedException(
                                    $"The generator of type '{generator.GetType()}' is not supported as a part of MultipleOutputGenerator." +
                                    "Only IFileGenerator and IInPlaceGenerator are suppored."
                                );
                        }
                    }
                );

                if (!executed)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool TryGenerateFiles(
            ISolution solution, 
            List<FileGeneratorOutput> fileGeneratorOutputs,
            List<InPlaceGeneratorOutput> inPlaceGeneratorOutputs)
        {
            if (!TryProcessFilePaths(solution, fileGeneratorOutputs, out List<FileGeneratorOutput> notFoundProjectFolderOutputs))
            {
                return false;
            }

            using (var progressIndicator = NullProgressIndicator.Create())
            {
                IProjectModelTransactionCookie transaction =
                    solution.CreateTransactionCookie(DefaultAction.Rollback, "T4 Generating files", progressIndicator);
                using (transaction)
                {
                    if (!TryCreateProjectFolders(notFoundProjectFolderOutputs, transaction))
                    {
                        return false;
                    }

                    if (!TryCreateFiles(fileGeneratorOutputs, transaction))
                    {
                        return false;
                    }

                    MakeDocumentChanges(solution, fileGeneratorOutputs, inPlaceGeneratorOutputs);

                    transaction.Commit(progressIndicator);
                }
            }

            return true;
        }

        private static bool TryProcessFilePaths(
            ISolution solution, 
            List<FileGeneratorOutput> fileGeneratorOutputs,
            out List<FileGeneratorOutput> notFoundProjectFolderOutputs)
        {
            var stopExecutionReasons = new HashSet<string>();
            notFoundProjectFolderOutputs = new List<FileGeneratorOutput>();

            foreach (FileGeneratorOutput output in fileGeneratorOutputs)
            {
                string failReason;

                if (!SolutionFilePathHelpers.TryParseFilePath(output.FilePath, out RelativePath relativeFilePath, out failReason))
                {
                    stopExecutionReasons.Add(failReason);
                    continue;
                }

                output.FileName = relativeFilePath.Name;

                if (!SolutionFilePathHelpers.TryGetProject(solution, relativeFilePath, out IProject project, out failReason))
                {
                    stopExecutionReasons.Add(failReason);
                    continue;
                }

                output.Project = project;

                if (stopExecutionReasons.Count > 0)
                {
                    continue;
                }

                output.SubFolderRelativePath =
                    SolutionFilePathHelpers.GetProjectSubFolderRelativePath(relativeFilePath, project);

                if (SolutionFilePathHelpers.TryGetProjectFolder(output.SubFolderRelativePath, project, out IProjectFolder projectFolder))
                {
                    output.ProjectFolder = projectFolder;
                }
                else
                {
                    notFoundProjectFolderOutputs.Add(output);
                }
            }

            if (stopExecutionReasons.Count > 0)
            {
                MessageBox.ShowError(string.Join(Environment.NewLine, stopExecutionReasons));

                return false;
            }

            return true;
        }

        private static bool TryCreateFiles(
            List<FileGeneratorOutput> fileGeneratorOutputs, 
            IProjectModelTransactionCookie transaction)
        {
            var alreadyExistedFilePaths = new List<string>();

            foreach (FileGeneratorOutput output in fileGeneratorOutputs)
            {
                IProjectFile file = output.ProjectFolder.GetSubFiles(output.FileName).FirstOrDefault();
                if (file == null)
                {
                    file = ExecutionHelpers.CreateFile(
                        output.FileName,
                        output.ProjectFolder,
                        output.SourceProjectFile,
                        transaction
                    );
                }
                else
                {
                    alreadyExistedFilePaths.Add(output.FilePath);
                }

                output.GeneratedProjectFile = file;
            }

            if (alreadyExistedFilePaths.Count > 0)
            {
                string message = 
                    $"The following files already exist:{Environment.NewLine}" +
                    string.Join(Environment.NewLine, alreadyExistedFilePaths) + Environment.NewLine + Environment.NewLine +
                    "Do you want to overwrite them?";

                return MessageBox.ShowYesNo(message);
            }

            return true;
        }

        private static bool TryCreateProjectFolders(
            List<FileGeneratorOutput> notFoundProjectFolderOutputs,
            IProjectModelTransactionCookie transaction)
        {
            if (notFoundProjectFolderOutputs.Count == 0)
            {
                return true;
            }

            IEnumerable<string> projectFolderPaths = notFoundProjectFolderOutputs
                .Select(x => (project: x.Project, folderPath: x.SubFolderRelativePath))
                .Distinct()
                .Select(x => RelativePath.Parse(x.project.Name).Combine(x.folderPath).ToString());

            string message =
                $"The following folders do not exist:{Environment.NewLine}" +
                string.Join(Environment.NewLine, projectFolderPaths) + Environment.NewLine + Environment.NewLine +
                "Do you want to create them?";


            if (!MessageBox.ShowYesNo(message))
            {
                return false;
            }

            foreach (FileGeneratorOutput output in notFoundProjectFolderOutputs)
            {
                FileSystemPath fileSystemPath = 
                    output.SubFolderRelativePath.MakeAbsoluteBasedOn(output.Project.Location);
                output.ProjectFolder = output.Project.GetOrCreateProjectFolder(fileSystemPath, transaction);
            }

            return true;
        }

        private static void MakeDocumentChanges(
            ISolution solution,
            List<FileGeneratorOutput> fileGeneratorOutputs,
            List<InPlaceGeneratorOutput> inPlaceGeneratorOutputs)
        {
            foreach (FileGeneratorOutput output in fileGeneratorOutputs)
            {
                IDocument document = output.GeneratedProjectFile.GetDocument();
                document.ReplaceText(document.DocumentRange, output.GeneratedText);
                ExecutionHelpers.OrginizeUsingsAndFormatFile(solution, document);
            }

            foreach (InPlaceGeneratorOutput output in inPlaceGeneratorOutputs)
            {
                IDocument document = output.ProjectFile.GetDocument();
                if (!output.RangeToDelete.IsEmpty)
                {
                    document.DeleteText(output.RangeToDelete);
                }

                document.InsertText(output.PositionToInsert, output.GeneratedText);

                var treeTextRange = TreeTextRange.FromLength(new TreeOffset(output.PositionToInsert), output.GeneratedText.Length);
                ExecutionHelpers.FormatFileRangeAndAddUsingDirectives(solution, document, treeTextRange, output.MissingUsingDirectives);
            }
        }

        private static void NavigateToFiles(
            ISolution solution,
            List<FileGeneratorOutput> fileGeneratorOutputs,
            List<InPlaceGeneratorOutput> inPlaceGeneratorOutputs)
        {
            NavigationManager navigationManager = NavigationManager.GetInstance(solution);

            var navigationOptions = NavigationOptions.FromWindowContext(
                Shell.Instance.GetComponent<IMainWindowPopupWindowContext>().Source,
                "",
                false /* activate */
            );

            foreach (FileGeneratorOutput output in fileGeneratorOutputs)
            {
                var navigationPoint = new ProjectFileNavigationPoint(output.GeneratedProjectFile);
                navigationManager.Navigate(navigationPoint, navigationOptions);
            }

            foreach (InPlaceGeneratorOutput output in inPlaceGeneratorOutputs)
            {
                var navigationPoint = new ProjectFileNavigationPoint(output.ProjectFile);
                navigationManager.Navigate(navigationPoint, navigationOptions);
            }
        }
    }
}