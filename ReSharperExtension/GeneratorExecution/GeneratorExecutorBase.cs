using T4CodeGenerator.Generators.Core.Contracts;

namespace T4CodeGenerator.ReSharperExtension.GeneratorExecution
{
    public abstract class GeneratorExecutorBase<TIGenerator> : IGeneratorExecutor
        where TIGenerator : IGenerator
    {
        public bool IsApplicable(IGenerator generator)
        {
            return generator is TIGenerator;
        }

        public bool TryExecute(IGenerator generator, GeneratorExecutionHost executionHost)
        {
            return TryExecute((TIGenerator) generator, executionHost);
        }

        protected abstract bool TryExecute(TIGenerator generator, GeneratorExecutionHost executionHost);
    }
}