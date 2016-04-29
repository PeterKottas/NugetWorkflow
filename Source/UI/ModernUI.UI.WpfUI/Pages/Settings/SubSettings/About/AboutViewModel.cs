using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.UI.WpfUI.Attributes;
using NugetWorkflow.UI.WpfUI.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.UI.WpfUI.Pages.Settings.SubSettings.About
{
    [SaveConfigAttribute]
    public class AboutViewModel : BaseViewModel, IViewModel
    {
        public AboutViewModel()
        {
            Initialize();
        }

        public void Initialize()
        {
        }
    }
}
