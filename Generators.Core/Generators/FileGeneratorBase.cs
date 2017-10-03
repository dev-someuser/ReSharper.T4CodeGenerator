using System.Collections.Generic;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;

using T4CodeGenerator.Generators.Core.Contracts;
using T4CodeGenerator.Generators.Core.Helpers;

namespace T4CodeGenerator.Generators.Core.Generators
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public abstract class FileGeneratorBase : GeneratorBase, IFileGenerator
    {
        private readonly HashSet<string> _importedNamespaces = new HashSet<string>();

        public abstract string GetFileName();

        protected string CurrentProject => this.DataProvider.Project.Name;

        protected string CurrentFolder
        {
            get
            {
                IProjectFolder projectFolder = this.DataProvider.SourceFile.ToProjectFile()?.ParentFolder;
                if (projectFolder != null && !Equals(projectFolder, this.DataProvider.Project))
                {
                    RelativePath projectPath = RelativePath.Parse(this.DataProvider.Project.Name);
                    RelativePath folderPath = projectFolder.Location.MakeRelativeTo(this.DataProvider.Project.Location);
                    return projectPath.Combine(folderPath).ToString();
                }

                return this.DataProvider.Project.Name;
            }
        }

        protected string ExpectedNamespace
        {
            get
            {
                var gotProjectFolder = SolutionFilePathHelpers.TryGetProjectFolderByFilePath(
                    this.GetFileName(),
                    this.DataProvider.Solution,
                    out IProjectFolder projectFolder
                );

                if (gotProjectFolder)
                {
                    return projectFolder.CalculateExpectedNamespace(CSharpLanguage.Instance);
                }

                return null;
            }
        }

        protected override void ProcessImportedNamespace(string @namespace)
        {
            if (!string.IsNullOrEmpty(@namespace) && _importedNamespaces.Add(@namespace))
            {
                this.GenerationEnvironment.Insert(0, $"using {@namespace};");
            }
        }
    }
}