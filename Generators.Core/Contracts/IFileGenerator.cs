namespace T4CodeGenerator.Generators.Core.Contracts
{
    public interface IFileGenerator : IGenerator
    {
        string GetFileName();

        string TransformText();
    }
}