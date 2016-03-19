using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.UI.WpfUI.Pages.Home
{
    public class HomePageViewModel : NotifyPropertyChanged
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
