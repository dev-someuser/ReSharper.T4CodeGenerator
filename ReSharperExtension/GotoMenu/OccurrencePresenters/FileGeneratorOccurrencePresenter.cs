using System;
using System.Drawing;

using JetBrains.Application.UI.Controls.JetPopupMenu;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.UnitTestFramework.Resources;
using JetBrains.UI.RichText;

using T4CodeGenerator.Generators.Core.Contracts;

namespace T4CodeGenerator.ReSharperExtension.GotoMenu.OccurrencePresenters
{
    [OccurrencePresenter]
    public class FileGeneratorOccurrencePresenter : IOccurrencePresenter
    {
        public bool Present(
            IMenuItemDescriptor descriptor, 
            IOccurrence occurrence,
            OccurrencePresentationOptions occurrencePresentationOptions)
        {
            var generatorOccurrence = (GeneratorOccurrence)occurrence;
            var generator = (IFileGenerator)generatorOccurrence.Generator;

            descriptor.Text = generator.Name;
            descriptor.Style = MenuItemStyle.Enabled;
            descriptor.Icon = UnitTestingThemedIcons.NewSession.Id;

            string assemblyName = generator.GetType().Assembly.GetName().Name;
            descriptor.ShortcutText = new RichText(assemblyName, TextStyle.FromForeColor(SystemColors.GrayText));

            descriptor.Tooltip = new RichText();
            if (!string.IsNullOrWhiteSpace(generator.Description))
            {
                descriptor.Tooltip.Append(generator.Description);
                descriptor.Tooltip.Append(Environment.NewLine);
            }

            descriptor.Tooltip.Append(new RichText("Output: ", new TextStyle(FontStyle.Bold)));
            descriptor.Tooltip.Append(new RichText(generator.GetFileName(), TextStyle.Default));

            return true;
        }

        public bool IsApplicable(IOccurrence occurrence)
        {
            return
                occurrence is GeneratorOccurrence generatorOccurrence &&
                generatorOccurrence.Generator is IFileGenerator;
        }
    }
}