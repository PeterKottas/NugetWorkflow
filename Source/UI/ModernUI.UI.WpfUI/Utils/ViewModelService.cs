using NugetWorkflow.Common.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NugetWorkflow.UI.WpfUI.Utils
{
    public static class ViewModelService
    {
        public static RET GetViewModel<RET>()
        {
            return ((App)Application.Current).GetView<RET>();
        }

        public static object GetViewModel(Type type)
        {
            return ((App)Application.Current).GetView(type);
        }
    }
}
