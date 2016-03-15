using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NugetWorkflow.UI.WpfUI.Pages
{
    public class CloneProjectsViewModel : NotifyPropertyChanged
    {
        private List<GitRepoViewModelDTO> testList;

        public List<GitRepoViewModelDTO> TestList
        {
            get
            {
                return this.testList;
            }
            set
            {
                testList = value;
                OnPropertyChanged("TestList");
            }
        }

        public CloneProjectsViewModel()
        {
            testList = new List<GitRepoViewModelDTO>();
            testList.Add(new GitRepoViewModelDTO() { Url = "test" });
        }
    }
}
