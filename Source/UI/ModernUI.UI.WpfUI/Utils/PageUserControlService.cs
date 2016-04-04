using NugetWorkflow.Common.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.UI.WpfUI.Utils
{
    public static class PageUserControlService
    {
        private static List<object> pageUserControls = new List<object>();

        public static void AddUserControl(object userControl)
        {
            pageUserControls.Add(userControl);
        }

        public static void ReassignViewModels()
        {
            foreach (var pageUserControl in pageUserControls)
            {
                if(pageUserControl is IPageUserControl)
                {
                    ((IPageUserControl)pageUserControl).AssignViewModel();
                }
            }
        }
    }
}
