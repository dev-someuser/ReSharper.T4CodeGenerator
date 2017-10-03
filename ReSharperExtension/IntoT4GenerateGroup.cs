using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.ReSharper.Feature.Services.Menu;

namespace T4CodeGenerator.ReSharperExtension
{
    [ActionGroup(ActionGroupInsertStyles.Embedded)]
    public class IntoT4GenerateGroup : IAction, IInsertFirst<GenerateGroup>
    {
        public IntoT4GenerateGroup(T4GenerateAction t4GenerateAction)
        {
            
        }
    }
}