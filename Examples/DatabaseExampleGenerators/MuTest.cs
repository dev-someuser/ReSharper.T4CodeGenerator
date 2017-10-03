using System.Collections.Generic;

using T4CodeGenerator.Generators.Core.Contracts;
using T4CodeGenerator.Generators.Core.Generators.Common;

namespace DatabaseExampleGenerators
{
    public class MuTest : ByClassMultipleOutputGeneratorBase
    {
        public override IEnumerable<IGenerator> GetGenerators()
        {
            yield return CreateChildGenerator<MigrationGenerator>(this.DataProvider.SelectedElement);
            yield return CreateChildGenerator<MigrationGenerator>(this.DataProvider.SelectedElement);
        }

        public override string Name => "MuTest";
    }
}