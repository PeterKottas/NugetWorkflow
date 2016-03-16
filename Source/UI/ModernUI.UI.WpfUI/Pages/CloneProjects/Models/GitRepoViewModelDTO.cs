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
                OnPropertyChanged("Status");
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

        public string Status
        {
            get 
            {
                Uri uriResult;
                bool result = Uri.TryCreate(Url, UriKind.Absolute, out uriResult);
                if(result)
                {
                    var matchGroups = Regex.Match(Url, @"(?'protocol'git@|https?:\/\/)(?'domain'[a-zA-Z0-9\.\-_]+)(\/|:)(?'group'[a-zA-Z0-9\-]+)\/(?'project'[a-zA-Z0-9\-]+)\.git").Groups;
                    if (matchGroups != null && matchGroups.Count>0)
                    {
                        var folderName = matchGroups[matchGroups.Count - 1].Value;
                        return folderName;
                    }
                    else
                    {
                        return "Can't get repo name";
                    }
                }
                else
                {
                    return "Wrong url format";
                }
            }
        }

        public GitRepoViewModelDTO()
        {
            hash = Guid.NewGuid().ToString();
        }
    }
}
