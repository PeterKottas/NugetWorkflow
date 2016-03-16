using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

namespace NugetWorkflow.UI.WpfUI.Pages
{
    public class GitRepoViewModelDTO : NotifyPropertyChanged
    {
        private SecureString _logonPassword;
        private string url;
        private string username;
        private string password;
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
                OnPropertyChanged("LogonPassword");
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

        public SecureString LogonPassword
        {
            get
            {
                return _logonPassword;
            }
            set
            {
                _logonPassword = value;
                OnPropertyChanged("LogonPassword");
            }
        }
        public GitRepoViewModelDTO()
        {
            hash = Guid.NewGuid().ToString();
        }
    }
}
