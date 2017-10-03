using System;
using System.Collections.Generic;

using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

using T4CodeGenerator.Generators.Core;
using T4CodeGenerator.Generators.Core.Contracts;

namespace T4CodeGenerator.ReSharperExtension.GeneratorsSource
{
    public class ApplicableGeneratorsCache
    {
        private readonly GeneratorsProvider _generatorsProvider;
        private readonly IGeneratorDataProvider _generatorDataProvider;
        private List<IGenerator> _generators;
        
        public ApplicableGeneratorsCache(
            GeneratorsProvider generatorsProvider,
            IGeneratorDataProvider generatorDataProvider)
        {
            _generatorsProvider = generatorsProvider;
            _generatorDataProvider = generatorDataProvider;
        }

        public IReadOnlyList<IGenerator> GetApplicableGenerators()
        {
            return _generators = _generators ?? GetApplicableGeneratorsFromProvider();
        }

        private List<IGenerator> GetApplicableGeneratorsFromProvider()
        {
            var result = new List<IGenerator>();

            foreach (IGenerator generator in _generatorsProvider.GetAllGenerators())
            {
                if (generator.Visible)
                {
                    generator.DataProvider = _generatorDataProvider;

                    using (CompilationContextCookie.GetOrCreate(generator.DataProvider.PsiFile.GetResolveContext()))
                    {
                        bool generatorAvailable = false;

                        try
                        {
                            generatorAvailable = generator.IsAvailable();
                        }
                        catch (Exception e)
                        {
                            // Pass to exceptions.
                        }

                        if (generatorAvailable)
                        {
                            result.Add(generator);
                        }
                    }
                }
            }

            return result;
        }
    }
}