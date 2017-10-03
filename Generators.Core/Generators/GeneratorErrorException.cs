using System;

namespace T4CodeGenerator.Generators.Core.Generators
{
    public class GeneratorErrorException : Exception
    {
        public GeneratorErrorException(string message) : base(message)
        {
            
        }
    }
}