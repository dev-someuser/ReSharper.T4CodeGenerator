using System;
using System.Collections.Generic;

using JetBrains.Application.Progress;
using JetBrains.DocumentManagers;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Transactions;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using JetBrains.Util.Extension;

using T4CodeGenerator.Generators.Core.Contracts;
using T4CodeGenerator.Generators.Core.Generators;

namespace T4CodeGenerator.ReSharperExtension.GeneratorExecution.Helpers
{
    public class ExecutionHelpers
    {
        public static bool TryExecuteGenerator(
            IFileGenerator generator, 
            out string generatedText, 
            out string filePath)
        {
            string localGeneratedText = null;
            string localFilePath = null;

            bool result = TryExecuteGenerator(
                generator,
                () =>
                {
                    localGeneratedText = generator.TransformText();
                    localFilePath = generator.GetFileName();
                }
            );

            generatedText = localGeneratedText;
            filePath = localFilePath;

            return result;
        }

        public static bool TryExecuteGenerator(
            IInPlaceGenerator generator,
            out string generatedText,
            out TextRange rangeToDelete,
            out int positionToInsert,
            out IReadOnlyCollection<IUsingDirective> missingUsingDirectives)
        {
            string localGeneratedText = null;
            TextRange localRangeToDelete = new TextRange(0);
            int localPositionToInsert = 0;
            IReadOnlyCollection<IUsingDirective> localMissingUsingDirectives = null;

            bool result = TryExecuteGenerator(
                generator,
                () =>
                {
                    localGeneratedText = generator.TransformText();
                    localRangeToDelete = generator.GetTextRangeToDelete();
                    localPositionToInsert = generator.GetPositionToInsert();
                    localMissingUsingDirectives = generator.MissingUsingDirectives;
                }
            );

            generatedText = localGeneratedText;
            rangeToDelete = localRangeToDelete;
            positionToInsert = localPositionToInsert;
            missingUsingDirectives = localMissingUsingDirectives;

            return result;
        }

        public static bool TryExecuteGenerator(IGenerator generator, Action executeAction)
        {
            using (CompilationContextCookie.GetOrCreate(generator.DataProvider.PsiFile.GetResolveContext()))
            {
                try
                {
                    executeAction();
                    return true;
                }
                catch (GeneratorErrorException e)
                {
                    MessageBox.ShowError(e.Message);
                }
                catch (Exception e)
                {
                    MessageBox.ShowError(e.ToString());
                }
            }

            return false;
        }

        public static void OrginizeUsingsAndFormatFile(ISolution solution, IDocument document)
        {
            IPsiSourceFile sourceFile = document.GetPsiSourceFile(solution);
            IPsiServices psiServices = solution.GetPsiServices();

            psiServices.Files.CommitAllDocuments();

            PsiTransactionCookie.ExecuteConditionally(
                psiServices,
                () =>
                {
                    if (sourceFile?.GetPsiFile(CSharpLanguage.Instance, new DocumentRange(document, 0)) is ICSharpFile psiFile)
                    {
                        SortImports(psiFile);

                        psiFile.OptimizeImportsAndRefs(
                            new RangeMarker(document.ToPointer(), new TextRange(0, document.DocumentRange.Length)),
                            true /* optimizeUsings */,
                            true /* shortenReferences */,
                            NullProgressIndicator.Create()
                        );

                        psiFile.FormatNode();
                    }

                    return true;
                },
                "Format T4 Generated File"
            );
        }

        public static void FormatFileRangeAndAddUsingDirectives(
            ISolution solution, 
            IDocument document, 
            TreeTextRange treeTextRange,
            IReadOnlyCollection<IUsingDirective> usingDirectives)
        {
            IPsiSourceFile sourceFile = document.GetPsiSourceFile(solution);
            IPsiServices psiServices = solution.GetPsiServices();

            psiServices.Files.CommitAllDocuments();

            PsiTransactionCookie.ExecuteConditionally(
                psiServices,
                () =>
                {
                    if (sourceFile?.GetPsiFile(CSharpLanguage.Instance, new DocumentRange(document, 0)) is ICSharpFile psiFile)
                    {
                        psiFile.FormatFileRange(treeTextRange);

                        foreach (IUsingDirective usingDirective in usingDirectives)
                        {
                            psiFile.AddImport(usingDirective);
                        }
                    }
                    
                    return true;
                },
                "Format T4 Generated File"
            );
        }

        public static IProjectFile CreateFile(
            string fileName, 
            IProjectFolder projectFolder, 
            IProjectFile sourceFile,
            IProjectModelTransactionCookie transaction)
        {
            FileSystemPath fileLocation = projectFolder.Location.Combine(fileName);

            IProjectFile file = transaction.AddFile(projectFolder, fileLocation);

            IDocument sourceDocument = sourceFile.GetDocument();
            IDocument document = file.GetDocument();
            document.Encoding = sourceDocument.Encoding;
            file.Properties.BuildAction = sourceFile.Properties.BuildAction;

            return file;
        }

        private static void SortImports(ICSharpFile psiFile)
        {
            IUsingDirective[] imports = psiFile.Imports.ToArray();

            string defaultNamespaceFirstPart = null;
            IProject project = psiFile.GetSourceFile().ToProjectFile()?.GetProject();
            if (project?.ProjectProperties is CSharpProjectProperties projectProperties)
            {
                defaultNamespaceFirstPart = projectProperties.CSharpBuildSettings.DefaultNamespace.SubstringBefore(".");
            }

            imports.Sort((x, y) => CompareUsingDirectives(x, y, defaultNamespaceFirstPart));

            foreach (IUsingDirective import in imports)
            {
                psiFile.RemoveImport(import);
            }

            foreach (IUsingDirective import in imports)
            {
                psiFile.AddImport(import);
            }
        }

        private static int CompareUsingDirectives(
            IUsingDirective left,
            IUsingDirective right,
            string defaultNamespaceFirstPart)
        {
            // Order:
            // 1. "System" namespaces.
            // 2. Mine namespaces.
            // 3. Other namespaces.
            // 4. Aliases.

            int GetUsingDirectivePriority(IUsingDirective usingDirective)
            {
                const string system = "System";

                if (usingDirective is IUsingSymbolDirective usingSymbolDirective)
                {
                    string qualifiedName = usingSymbolDirective.ImportedSymbolName.QualifiedName;

                    if (qualifiedName.StartsWith($"{system}.") || qualifiedName == system)
                    {
                        return 0;
                    }

                    if (!string.IsNullOrEmpty(defaultNamespaceFirstPart) && qualifiedName.StartsWith(defaultNamespaceFirstPart))
                    {
                        return 2;
                    }

                    return 1;
                }

                return 3;
            }

            return GetUsingDirectivePriority(left).CompareTo(GetUsingDirectivePriority(right));
        }
    }
}