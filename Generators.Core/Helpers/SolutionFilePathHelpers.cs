using System;
using System.Linq;

using JetBrains.ProjectModel;
using JetBrains.Util;

namespace T4CodeGenerator.Generators.Core.Helpers
{
    public static class SolutionFilePathHelpers
    {
        public static bool TryGetProjectFolderByFilePath(
            string filePath, 
            ISolution solution,
            out IProjectFolder projectFolder)
        {
            projectFolder = null;

            if (!TryParseFilePath(filePath, out RelativePath relativeFilePath, out _))
            {
                return false;
            }

            if (!TryGetProject(solution, relativeFilePath, out IProject project, out _))
            {
                return false;
            }

            if (!TryGetProjectFolder(GetProjectSubFolderRelativePath(relativeFilePath, project), project, out projectFolder))
            {
                return false;
            }

            return true;
        }

        public static bool TryParseFilePath(
            string filePath,
            out RelativePath relativeFilePath,
            out string failReason)
        {
            relativeFilePath = RelativePath.TryParse(filePath);
            if (relativeFilePath == RelativePath.Empty)
            {
                failReason =
                    $"Invalid file path '{filePath}'. The path must start with project name ('ProjectName\\...\\FileName.ext').";
                return false;
            }

            failReason = null;
            return true;
        }

        public static bool TryGetProject(
            ISolution solution,
            RelativePath relativeFilePath,
            out IProject project,
            out string failReason)
        {
            string projectName = relativeFilePath.FirstComponent;

            project = solution.GetAllProjects()
                .FirstOrDefault(x => string.Equals(x.Name, projectName, StringComparison.OrdinalIgnoreCase));

            if (project == null)
            {
                failReason = $"Invalid file path '{relativeFilePath}'. The prject '{projectName}' is not found.";
                return false;
            }

            failReason = null;
            return true;
        }

        public static bool TryGetProjectFolder(
            RelativePath folderRelativePath,
            IProject project,
            out IProjectFolder projectFolder)
        {
            projectFolder = project;

            if (folderRelativePath != RelativePath.Empty)
            {
                projectFolder = project;
                foreach (string folderName in folderRelativePath.GetPathComponents())
                {
                    projectFolder = projectFolder.GetSubFolders(folderName).FirstOrDefault();
                    if (projectFolder == null)
                    {
                        break;
                    }
                }
            }

            return projectFolder != null;
        }

        public static RelativePath GetProjectSubFolderRelativePath(
            RelativePath relativeFilePath,
            IProject project)
        {
            return relativeFilePath.TryMakeRelativeTo(RelativePath.Parse(project.Name)).Parent;
        }
    }
}