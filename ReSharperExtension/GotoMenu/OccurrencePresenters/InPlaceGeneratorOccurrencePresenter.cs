using System.Drawing;

using JetBrains.Application.UI.Controls.JetPopupMenu;
using JetBrains.Application.UI.Options.Options.ThemedIcons;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.UI.RichText;

using T4CodeGenerator.Generators.Core.Contracts;

namespace T4CodeGenerator.ReSharperExtension.GotoMenu.OccurrencePresenters
{
    [OccurrencePresenter]
    public class InPlaceGeneratorOccurrencePresenter : IOccurrencePresenter
    {
        public bool Present(
            IMenuItemDescriptor descriptor, 
            IOccurrence occurrence,
            OccurrencePresentationOptions occurrencePresentationOptions)
        {
            var generatorOccurrence = (GeneratorOccurrence) occurrence;
            var generator = (IInPlaceGenerator)generatorOccurrence.Generator;

            descriptor.Text = generator.Name;
            descriptor.Style = MenuItemStyle.Enabled;
            descriptor.Icon = OptionsThemedIcons.ImportLayer.Id;
            descriptor.Tooltip = generator.Description;

            string assemblyName = generator.GetType().Assembly.GetName().Name;
            descriptor.ShortcutText = new RichText(assemblyName, TextStyle.FromForeColor(SystemColors.GrayText));

            return true;
        }

        public bool IsApplicable(IOccurrence occurrence)
        {
            return
                occurrence is GeneratorOccurrence generatorOccurrence &&
                generatorOccurrence.Generator is IInPlaceGenerator;
        }
    }
}