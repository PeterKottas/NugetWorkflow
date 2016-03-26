using FirstFloor.ModernUI.Presentation;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.BaseSetup;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Converters;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Shared.Enums;
using NugetWorkflow.UI.WpfUI.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using NugetWorkflow.Common.Base.Extensions;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Extensions;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Models
{
    public class GitRepoModel : NotifyPropertyChanged
    {
        private SecureString password;
        private string url;
        private string username;
        private string hash;
        private bool useOverrideCredentials;
        private bool cloneToogle;
        private bool updateToogle;
        private CloneStatusEnum cloneStatus;

        public bool UpdateToggle
        {
            get
            {
                return updateToogle;
            }
            set
            {
                updateToogle = value;
                OnPropertyChanged("UpdateToggle");
            }
        }
        
        public bool CloneToggle
        {
            get
            {
                return cloneToogle;
            }
            set
            {
                cloneToogle = value;
                OnPropertyChanged("CloneSelected");
            }
        }

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
                CloneStatus = GetStatus();
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

        public bool UseOverrideCredentials
        {
            get
            {
                return useOverrideCredentials;
            }
            set
            {
                useOverrideCredentials = value;
                OnPropertyChanged("UseOverrideCredentials");
            }
        }

        public string CloneStatusMessage
        {
            get
            {
                return CloneStatus.ToUserFriendlyMessage();
            }
        }

        public CloneStatusEnum CloneStatus
        {
            get
            {
                return cloneStatus;
            }
            set
            {
                cloneStatus = value;
                OnPropertyChanged("CloneStatus");
                OnPropertyChanged("CloneStatusMessage");
            }
        }

        public void UpdateStatus()
        {
            CloneStatus = GetStatus();
        }

        private CloneStatusEnum GetStatus()
        {
            string repoUrl = Url;
            string basePath = ViewModelService.GetViewModel<BaseSetupViewModel>().BasePath;

            if(string.IsNullOrEmpty(basePath))
            {
                return CloneStatusEnum.BasePathUndefined;
            }
            try 
	        {
                Path.GetFullPath(basePath);
	        }
	        catch (Exception)
	        {
                return CloneStatusEnum.BasePathWrongFormat;
	        }

            Uri uriResult;
            bool result = Uri.TryCreate(repoUrl, UriKind.Absolute, out uriResult);
            if (result)
            {
                var matchGroups = Regex.Match(repoUrl, @"([^/]+)\.git").Groups;
                string repoName = string.Empty;
                if (repoUrl.GetFolderFromUrl(ref repoName))
                {
                    if(string.IsNullOrEmpty(repoName))
                    {
                        return CloneStatusEnum.WrongUrlFormat;
                    }
                    if (!Directory.Exists(Path.Combine(basePath, repoName)))
                    {
                        return CloneStatusEnum.OK;
                    }
                    return CloneStatusEnum.AlreadyExists;
                }
                else
                {
                    return CloneStatusEnum.WrongUrlFormat;
                }
            }
            else
            {
                return CloneStatusEnum.WrongUrlFormat;
            }
        }

        public GitRepoModel()
        {
            hash = Guid.NewGuid().ToString();
            useOverrideCredentials = false;
            username = string.Empty;
            cloneToogle = false;
            url = string.Empty;
        }
    }
}
