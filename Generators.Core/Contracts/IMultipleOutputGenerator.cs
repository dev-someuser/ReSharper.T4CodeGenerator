using System.Collections.Generic;

namespace T4CodeGenerator.Generators.Core.Contracts
{
    public interface IMultipleOutputGenerator : IGenerator
    {
        IEnumerable<IGenerator> GetGenerators();
    }
}