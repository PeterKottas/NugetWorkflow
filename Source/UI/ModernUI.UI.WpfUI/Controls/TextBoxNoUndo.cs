using NugetWorkflow.UI.WpfUI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace NugetWorkflow.UI.WpfUI.Controls
{
    public class TextBoxNoUndo : TextBox
    {
        public TextBoxNoUndo()
        {
            CommandBinding undoBinding = new CommandBinding(ApplicationCommands.Undo, null, new CanExecuteRoutedEventHandler(CanExecuteUndo));
            CommandBindings.Add(undoBinding);
            CommandBinding redoBinding = new CommandBinding(ApplicationCommands.Redo, null, new CanExecuteRoutedEventHandler(CanExecuteRedo));
            CommandBindings.Add(redoBinding);
            UndoLimit = 1;
        }

        private void CanExecuteUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            // Block TextBox from handling this command. 
            UndoManager.Undo();
            e.CanExecute = false;
            e.Handled = true;
        }

        private void CanExecuteRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            // Block TextBox from handling this command.  
            UndoManager.Redo();
            e.CanExecute = false;
            e.Handled = true;
        }
    } 
}
