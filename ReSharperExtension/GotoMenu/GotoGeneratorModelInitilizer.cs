using JetBrains.Application.UI.Controls.GotoByName;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation.Goto.ProvidersAPI;

namespace T4CodeGenerator.ReSharperExtension.GotoMenu
{
    [SolutionComponent]
    public class GotoGeneratorModelInitilizer : IModelInitializer
    {
        public void InitModel(Lifetime lifetime, GotoByNameModel model)
        {
            model.CaptionText.Value = "Enter generator name or it's type name:";
            model.IsCheckBoxVisible.Value = false;
            model.IsCheckBoxCheckerVisible.Value = false;
            model.NotReadyMessage.Value = "Preparing the list of items, please wait…";
            model.IsCheckBoxChecked.Value = true;
        }
    }
}