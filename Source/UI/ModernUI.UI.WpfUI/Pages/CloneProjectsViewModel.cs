using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NugetWorkflow.UI.WpfUI.Pages
{
    public class CloneProjectsViewModel : NotifyPropertyChanged
    {
        private List<TestDTO> testList;

        public List<TestDTO> TestList
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
            testList = new List<TestDTO>();
            testList.Add(new TestDTO() { Name = "test", ID=5 });
        }
    }
}
