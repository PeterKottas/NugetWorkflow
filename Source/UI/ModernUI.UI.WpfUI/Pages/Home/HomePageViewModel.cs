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
        private string header;

        #region Properties names
        public static readonly string HeaderPropName = ReflectionUtility.GetPropertyName((HomePageViewModel s) => s.Header);
        #endregion

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
        public HomePageViewModel()
        {
        }
    }
}
