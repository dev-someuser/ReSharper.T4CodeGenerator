using JetBrains.ReSharper.Feature.Services.Navigation.Goto.ProvidersAPI;

namespace T4CodeGenerator.ReSharperExtension.GotoMenu
{
    public interface IGotoGeneratorProvider : IInstantGotoGeneratorProvider, IOccurrenceNavigationProvider
    {
        
    }
}