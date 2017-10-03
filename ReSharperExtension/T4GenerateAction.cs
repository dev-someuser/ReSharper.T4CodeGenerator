using System;
using System.Diagnostics;
using System.Windows.Forms;

using JetBrains.Application.DataContext;
using JetBrains.Application.Threading;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.Components.UIApplication;
using JetBrains.Application.UI.Controls.GotoByName;
using JetBrains.Application.UI.DataContext;
using JetBrains.Application.UI.PopupLayout;
using JetBrains.DataFlow;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Feature.Services.Navigation.Goto.Misc;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.TextControl.Coords;
using JetBrains.TextControl.DataContext;
using JetBrains.UI.CommonControls;
using JetBrains.UI.Controls;
using JetBrains.UI.Controls.GotoByName;
using JetBrains.UI.SrcView.Controls.JetPopupMenu.Impl;
using JetBrains.UI.StdApplicationUI;
using JetBrains.Util;

using T4CodeGenerator.ReSharperExtension.GotoMenu;

namespace T4CodeGenerator.ReSharperExtension
{
    [Action(
        "&Generate using T4 template...", 
        Id = 9876, 
        IdeaShortcuts = new[] { "Alt+OemPlus" }, 
        VsShortcuts = new[] { "Alt+OemPlus" })
    ]
    public class T4GenerateAction : IExecutableAction
    {
        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            ISolution solution = context.GetData(ProjectModelDataConstants.SOLUTION);
            IShellLocks locks = context.GetComponent<IShellLocks>();
            PopupWindowContextSource popupWindowContextSource = context.GetData(UIDataConstants.PopupWindowContextSource);

            Debug.Assert(popupWindowContextSource != null, "popupWindowContextSource != null");
            Debug.Assert(solution != null, "solution != null");

            void Atomic(LifetimeDefinition lifetimeDefinition, Lifetime lifetime)
            {
                var controller = new GotoGeneratorController(
                    lifetime, 
                    solution, 
                    locks, 
                    context,
                    Shell.Instance.GetComponent<IMainWindowPopupWindowContext>(),
                    false /* enableMulticore */
                );

                GotoByNameMenu menu = new GotoByNameMenu(
                    context.GetComponent<GotoByNameMenuComponent>(),
                    lifetimeDefinition,
                    controller.Model,
                    context.GetComponent<UIApplication>().MainWindow.TryGetActiveWindow(),
                    solution.GetComponent<GotoByNameModelManager>().GetSearchTextData(context, controller),
                    popupWindowContextSource.Create(lifetime)
                );

                MakeBusyIconVisibleOnEmptyFilter(menu, controller.Model, lifetime);
            }

            Lifetimes.Define(solution.GetLifetime(), null /* id */, Atomic);
        }

        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            // Better to check everything now and then work with valid data.

            ITextControl textControl = context.GetData(TextControlDataConstants.TEXT_CONTROL);

            if (textControl == null)
            {
                return false;
            }

            if (textControl.IsReadOnly)
            {
                return false;
            }

            if (textControl.Selection.IsDisjoint())
            {
                return false;
            }

            IEquatableList<TextControlPosRange> equatableList = textControl.Selection.Ranges.Value;
            if (equatableList.Count > 1)
            {
                return false;
            }

            ISolution solution = context.GetData(ProjectModelDataConstants.SOLUTION);
            if (solution == null)
            {
                return false;
            }

            IPsiSourceFile psiSourceFile = textControl.Document.GetPsiSourceFile(solution);
            if (psiSourceFile == null)
            {
                return false;
            }

            if (!psiSourceFile.IsValid())
            {
                return false;
            }

            TextRange textRange =
                equatableList.Count == 1 ? equatableList[0].ToDocRangeNormalized() : new TextRange(textControl.Caret.Offset());
            DocumentRange range = new DocumentRange(textControl.Document, textRange);
            ICSharpFile psiFile = psiSourceFile.GetPsiFile<CSharpLanguage>(range) as ICSharpFile;
            if (psiFile == null)
            {
                return false;
            }

            if (!psiFile.IsValid())
            {
                return false;
            }

            if (context.GetData(UIDataConstants.PopupWindowContextSource) == null)
            {
                return false;
            }

            return true;
        }

        private static void MakeBusyIconVisibleOnEmptyFilter(GotoByNameMenu menu, GotoByNameModel model, Lifetime lifetime)
        {
            EditboxGlyph glyph = null;
            EditboxCueBanner banner = null;
            JetPopupMenuView view = menu.MenuView.GetValue();
            foreach (Control control in view.Title.QuickSearchEditbox.Controls)
            {
                glyph = glyph ?? control as EditboxGlyph;
                banner = banner ?? control as EditboxCueBanner;
                if (glyph != null && banner != null)
                {
                    break;
                }
            }

            IProperty<bool> property = model.IsReady.CreateNot(lifetime);
            IProperty<bool> target = new Property<bool>(lifetime, "isShowGlyphDeferred");
            PropertyBindingDeferred<bool> propertyBindingDeferred =
                new PropertyBindingDeferred<bool>(lifetime, property, target, TimeSpan.FromSeconds(0.3));
            target.Change.Advise_HasNew(lifetime, args => glyph.Visible = args.New);
            property.WhenTrueOnce(lifetime, () => banner.Visible = false);
        }
    }
}