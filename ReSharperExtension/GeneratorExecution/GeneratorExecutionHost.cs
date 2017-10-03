using System;
using System.Linq;

using JetBrains.Application;

using T4CodeGenerator.Generators.Core.Contracts;

namespace T4CodeGenerator.ReSharperExtension.GeneratorExecution
{
    [ShellComponent(Lifecycle.OneCall, Creation.AnyThread, Access.AnyThread)]
    public class GeneratorExecutionHost
    {
        private readonly IFeaturePartsContainer _featureParts;

        public GeneratorExecutionHost(IFeaturePartsContainer featureParts)
        {
            _featureParts = featureParts;
        }

        public bool TryExecute(IGenerator generator)
        {
            IGeneratorExecutor generatorExecutor = 
                _featureParts.GetFeatureParts<IGeneratorExecutor>(x => x.IsApplicable(generator)).FirstOrDefault();

            if (generatorExecutor != null)
            {
                return generatorExecutor.TryExecute(generator, this);
            }

            throw new InvalidOperationException($"There is no T4 generator executor for type '{generator.GetType()}'.");
        }
    }
}