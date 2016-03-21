using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace NugetWorkflow.UI.WpfUI.Pages.CloneProjects.Models
{
    public class GitRepoViewModelDTO : NotifyPropertyChanged
    {
        private SecureString password;
        private string url;
        private string username;
        private string hash;
        
        public string Hash
        {
            get
            {
                return hash;
            }
        }

        public string Url 
        { 
            get
            {
                return url;
            }
            set
            {
                url = value;
                OnPropertyChanged("Url");
            }
        }
        
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
                OnPropertyChanged("Username");
            }
        }

        public SecureString Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }

        public GitRepoViewModelDTO()
        {
            hash = Guid.NewGuid().ToString();
        }
    }
}
