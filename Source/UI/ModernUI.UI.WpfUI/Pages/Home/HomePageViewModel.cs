using FirstFloor.ModernUI.Presentation;
using NugetWorkflow.Common.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.UI.WpfUI.Pages.Home
{
    public class HomePageViewModel : NotifyPropertyChanged, IViewModel
    {
        private string header;
        public string Header
        {
            get
            {
                return header;
            }
            set
            {
                header = value;
                OnPropertyChanged("Header");
            }
        }
        public HomePageViewModel()
        {
            header = "test";
        }
    }
}
