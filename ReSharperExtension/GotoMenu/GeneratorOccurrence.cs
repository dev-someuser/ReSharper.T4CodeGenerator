using System;

using JetBrains.Application.UI.PopupLayout;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Occurrences;

using T4CodeGenerator.Generators.Core.Contracts;

namespace T4CodeGenerator.ReSharperExtension.GotoMenu
{
    public class GeneratorOccurrence : IOccurrence
    {
        public IGenerator Generator { get; }

        public bool IsValid => true;

        public OccurrencePresentationOptions PresentationOptions { get; set; } = OccurrencePresentationOptions.DefaultOptions;

        public OccurrenceType OccurrenceType => OccurrenceType.Occurrence;

        public ISolution GetSolution() => null;

        public GeneratorOccurrence(IGenerator generator)
        {
            Generator = generator;
        }

        public bool Navigate(
            ISolution solution, 
            PopupWindowContextSource windowContext, 
            bool transferFocus,
            TabOptions tabOptions = TabOptions.Default)
        {
            throw new NotImplementedException();
        }

        public string DumpToString()
        {
            // TODO.
            return string.Empty;
        }
    }
}