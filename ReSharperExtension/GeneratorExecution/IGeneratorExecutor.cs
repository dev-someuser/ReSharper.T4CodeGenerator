using T4CodeGenerator.Generators.Core.Contracts;

namespace T4CodeGenerator.ReSharperExtension.GeneratorExecution
{
    public interface IGeneratorExecutor
    {
        bool IsApplicable(IGenerator generator);

        bool TryExecute(IGenerator generator, GeneratorExecutionHost executionHost);
    }
}