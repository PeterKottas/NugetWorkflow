using NugetWorkflow.UI.WpfUI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NugetWorkflow.UI.WpfUI.Base
{
    public abstract class BaseUserControl<VIEWMODEL> : UserControl
    {
        void AssignViewModel()
        {
            this.DataContext = ViewModelService.GetViewModel<VIEWMODEL>();
        }
    }
}
