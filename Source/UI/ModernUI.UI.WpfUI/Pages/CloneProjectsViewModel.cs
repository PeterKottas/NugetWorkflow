using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows;

namespace NugetWorkflow.UI.WpfUI.Pages
{
    public class CloneProjectsViewModel : NotifyPropertyChanged
    {
        private ObservableCollection<GitRepoViewModelDTO> testList;

        public Thickness ButtonMargin
        {
            get
            {
                return new Thickness(5);
            }
        }
        public ObservableCollection<GitRepoViewModelDTO> TestList
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
            testList = new ObservableCollection<GitRepoViewModelDTO>();
            testList.Add(new GitRepoViewModelDTO() { Url = "test", });
        }
    }
}
