using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using JetBrains.Application.Progress;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ProjectModel.Properties.Managed;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Search;

using T4CodeGenerator.Generators.Core.Contracts;

namespace T4CodeGenerator.ReSharperExtension.GeneratorsSource
{
    [SolutionComponent]
    public class GeneratorsProvider
    {
        private readonly ISolution _solution;

        private readonly Dictionary<string, (Assembly assembly, DateTime lastWriteTime)> _fileNameToAssemblyLastWriteDateTimePairIndex = 
            new Dictionary<string, (Assembly, DateTime)>();

        private readonly Dictionary<Assembly, (string assemblyDirectory, Type[] loadedTypes)> _assemblyToDirectoryLoadedTypesPairIndex = 
            new Dictionary<Assembly, (string, Type[])>();

        public GeneratorsProvider(ISolution solution)
        {
            _solution = solution;
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
            solution.GetLifetime().AddAction(() => AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolve);
        }

        public IEnumerable<IGenerator> GetAllGenerators()
        {
            ReloadGeneratorTypes();

            foreach ((_, var loadedTypes) in _assemblyToDirectoryLoadedTypesPairIndex.Values)
            {
                foreach (Type generatorType in loadedTypes)
                {
                    var generator = (IGenerator)Activator.CreateInstance(generatorType);
                    yield return generator;
                }
            }
        }

        private void ReloadGeneratorTypes()
        {
            foreach (IProject project in GetProjectsContainingGenerators(_solution))
            {
                TargetFrameworkId frameworkId = project.ProjectProperties.PlatformId.ToTargetFrameworkId();
                var buildSettings = (IManagedProjectBuildSettings)project.ProjectProperties.BuildSettings;

                string assemblyDirectory = project.GetOutputDirectory(frameworkId).ToString();
                string assemblyFileName = buildSettings.GetOutputAssemblyFileName(frameworkId);
                string assemblyFilePath = Path.Combine(assemblyDirectory, assemblyFileName);

                if (!File.Exists(assemblyFilePath))
                {
                    continue;
                }

                DateTime lastWriteTime = File.GetLastWriteTime(assemblyFilePath);

                if (NeedToReloadAssembly(assemblyFilePath, lastWriteTime))
                {
                    TryReloadAssembly(assemblyFilePath, assemblyDirectory, lastWriteTime);
                }
            }
        }

        private bool TryReloadAssembly(string assemblyFilePath, string assemblyDirectory, DateTime lastWriteTime)
        {
            Assembly assembly;

            try
            {
                assembly = Assembly.Load(File.ReadAllBytes(assemblyFilePath));
            }
            catch (Exception e)
            {
                // Logging.
                return false;
            }

            if (_fileNameToAssemblyLastWriteDateTimePairIndex.TryGetValue(assemblyFilePath, out var assemblyLastWriteDateTimePair))
            {
                _assemblyToDirectoryLoadedTypesPairIndex.Remove(assemblyLastWriteDateTimePair.assembly);
            }

            _fileNameToAssemblyLastWriteDateTimePairIndex[assemblyFilePath] = (assembly, lastWriteTime);

            Type[] loadedTypes = assembly.GetExportedTypes()
                .Where(x => typeof(IGenerator).IsAssignableFrom(x))
                .ToArray();

            _assemblyToDirectoryLoadedTypesPairIndex.Add(assembly, (assemblyDirectory, loadedTypes));

            return true;
        }

        private bool NeedToReloadAssembly(string assemblyFilePath, DateTime lastWriteTime)
        {
            bool gotAssemblyLastWriteDateTimePair = _fileNameToAssemblyLastWriteDateTimePairIndex.TryGetValue(
                assemblyFilePath,
                out var assemblyLastWriteDateTimePair
            );

            if (!gotAssemblyLastWriteDateTimePair)
            {
                return true;
            }

            return lastWriteTime > assemblyLastWriteDateTimePair.lastWriteTime;
        }

        private Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.RequestingAssembly != null)
            {
                var gotFilePathLoadedTypesPair = _assemblyToDirectoryLoadedTypesPairIndex.TryGetValue(
                    args.RequestingAssembly,
                    out var assemblyDirectoryLoadedTypesPair
                );

                if (gotFilePathLoadedTypesPair)
                {
                    var assemblyName = new AssemblyName(args.Name);

                    string fileName = Path.Combine(assemblyDirectoryLoadedTypesPair.assemblyDirectory, assemblyName.Name) + ".dll";
                    if (File.Exists(fileName))
                    {
                        return Assembly.Load(File.ReadAllBytes(fileName));
                    }
                }
            }
            
            return null;
        }

        private static IEnumerable<IProject> GetProjectsContainingGenerators(ISolution solution)
        {
            ISymbolScope symbolScope = solution.GetPsiServices().Symbols.GetSymbolScope(LibrarySymbolScope.REFERENCED, true);
            if (symbolScope.GetElementsByQualifiedName(typeof(IGenerator).FullName).FirstOrDefault() is ITypeElement generatorType)
            {
                IPsiServices psiServices = solution.GetPsiServices();

                foreach (IProject project in solution.GetAllProjects())
                {
                    if (project.ProjectProperties.BuildSettings is CSharpBuildSettings)
                    {
                        ISearchDomain searchDomain = psiServices.SearchDomainFactory.CreateSearchDomain(project);
                        bool anyGeneratorFound = false;
                        psiServices.Finder.FindInheritors(
                            generatorType,
                            searchDomain,
                            new FindResultConsumer(
                                _ =>
                                {
                                    anyGeneratorFound = true;
                                    return FindExecution.Stop;
                                }
                            ),
                            NullProgressIndicator.Create()
                        );

                        if (anyGeneratorFound)
                        {
                            yield return project;
                        }
                    }
                }
            }
        }
    }
}