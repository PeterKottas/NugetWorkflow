using FirstFloor.ModernUI.Presentation;
using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.Common.Base.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.UI.WpfUI.Pages.Home
{
    public class HomePageViewModel : NotifyPropertyChanged, IViewModel
    {
        //Private properties
        private string header;
        //\Private properties

        //Properties names        
        public static readonly string HeaderPropName = ReflectionUtility.GetPropertyName((HomePageViewModel s) => s.Header);
        //\Properties names

        //Bindable properties
        public string Header
        {
            get
            {
                return header;
            }
            set
            {
                header = value;
                OnPropertyChanged(HeaderPropName);
            }
        }
        //\Bindable properties

        //Implementation
        public HomePageViewModel()
        {
        }

        public void Initialize()
        {
        }
        //\Implementation
    }
}
