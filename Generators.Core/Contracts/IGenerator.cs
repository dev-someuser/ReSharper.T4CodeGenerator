namespace T4CodeGenerator.Generators.Core.Contracts
{
    public interface IGenerator
    {
        IGeneratorDataProvider DataProvider { get; set; }

        string Name { get; }

        string Description { get; }

        bool Visible { get; }

        bool IsAvailable();
    }
}