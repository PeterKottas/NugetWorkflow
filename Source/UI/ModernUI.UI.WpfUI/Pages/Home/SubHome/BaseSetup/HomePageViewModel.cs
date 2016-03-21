using FirstFloor.ModernUI.Presentation;
using NugetWorkflow.UI.WpfUI.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.UI.WpfUI.Pages.Home
{
    public class HomePageViewModel : NotifyPropertyChanged, IView
    {
        private string basePath;
        public string BasePath
        {
            get
            {
                return basePath;
            }
            set
            {
                basePath = value;
                OnPropertyChanged("BasePath");
            }
        }
    }
}
