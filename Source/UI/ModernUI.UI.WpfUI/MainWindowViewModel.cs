using FirstFloor.ModernUI.Presentation;
using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.UI.WpfUI.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NugetWorkflow.UI.WpfUI
{
    public class MainWindowViewModel : BaseViewModel, IViewModel
    {
        //Data hiding
        private LinkCollection titleLinks;
        private string RepoUrl = "https://github.com/PeterKottas/NugetWorkflow";
        private string NewIssueUrl = "/issues/new";
        private string NewIssueQueryString = "?body={0}";
        private string NewBugMessage = "Your description here\n\n## Expected outcome\nWhat should happen?\n\n## Actual outcome\nWhat happened?\n\n## Versions\nApp: {0}\nWindows: {1}\n\n## Steps to reproduce\n1.\n2.\n3.";

        //\Data hiding
        //Bindable properties
        public string CurrentVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string WindowsVersion
        {
            get
            {
                return Environment.OSVersion.ToString();
            }
        }

        public LinkCollection TitleLinks
        {
            get
            {
                return titleLinks;
            }
        }
        //\Bindable properties 

        //Implementation
        public MainWindowViewModel()
        {
           
        }

        public void Initialize()
        {
            titleLinks = new LinkCollection();
            titleLinks.Add(new Link()
                {
                    DisplayName = "help",
                    Source = new Uri(RepoUrl)
                });
            titleLinks.Add(new Link()
                {
                    DisplayName = "bug",
                    Source = new Uri(RepoUrl + NewIssueUrl + string.Format(NewIssueQueryString, HttpUtility.UrlEncode(string.Format(NewBugMessage, CurrentVersion, WindowsVersion))))
                });
        }
        //\Implementation
    }
}
