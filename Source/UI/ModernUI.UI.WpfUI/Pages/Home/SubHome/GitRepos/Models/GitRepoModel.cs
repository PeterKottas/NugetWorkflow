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
        private bool isValidUrl = false;
        private string repoName = null;
        private string repoNameCustom = null;
        private bool useCustomRepoName = false;

        public bool UseDefaultRepoName
        {
            get 
            { 
                return !useCustomRepoName; 
            }
        }

        public bool UseCustomRepoName
        {
            get
            {
                return useCustomRepoName;
            }
            set
            {
                useCustomRepoName = value;
                OnPropertyChanged("UseCustomRepoName");
                OnPropertyChanged("UseDefaultRepoName");
                OnPropertyChanged("RepoName");
            }
        }

        public string RepoName
        {
            get
            {
                if (UseCustomRepoName)
                {
                    return repoNameCustom;
                }
                else
                {
                    return repoName;
                }
            }
            set
            {
                if (UseCustomRepoName)
                {
                    repoNameCustom = value;
                }
                else
                {
                    repoName = value;
                }
                OnPropertyChanged("RepoName");
            }
        }

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

        public bool IsValidUrl
        {
            get
            {
                return isValidUrl;
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
                Uri uriResult;
                bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult);
                if (result)
                {
                    string repoFolder = string.Empty;
                    url.GetFolderFromUrl(ref repoFolder);
                    RepoName = repoFolder;
                }
                else
                {
                    RepoName = null;
                }
                UpdateStatus();
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

        public void UpdateStatus(string basePath)
        {
            CloneStatus = GetStatus(basePath);
        }

        public void UpdateStatus()
        {
            var basePath = ViewModelService.GetViewModel<BaseSetupViewModel>().BasePath;
            UpdateStatus(basePath);
        }

        private CloneStatusEnum GetStatus(string basePath)
        {
            if (string.IsNullOrEmpty(basePath))
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
            if (string.IsNullOrEmpty(RepoName))
            {
                return CloneStatusEnum.WrongUrlFormat;
            }
            if (!Directory.Exists(Path.Combine(basePath, RepoName)))
            {
                return CloneStatusEnum.OK;
            }
            return CloneStatusEnum.AlreadyExists;
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
