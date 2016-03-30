using FirstFloor.ModernUI.Presentation;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.BaseSetup;
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
using NugetWorkflow.Common.Base.Utils;
using System.Linq.Expressions;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Models
{
    public class GitRepoModel : NotifyPropertyChanged
    {
        //Data hiding
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
        //\Data hiding

        //Properties names
        public static readonly string UseCustomRepoNamePropName = ReflectionUtility.GetPropertyName((GitRepoModel s) => s.UseCustomRepoName);
        
        public static readonly string UseDefaultRepoNamePropName = ReflectionUtility.GetPropertyName((GitRepoModel s) => s.UseDefaultRepoName);
        
        public static readonly string RepoNamePropName = ReflectionUtility.GetPropertyName((GitRepoModel s) => s.RepoName);
        
        public static readonly string UpdateTooglePropName = ReflectionUtility.GetPropertyName((GitRepoModel s) => s.UpdateToggle);
        
        public static readonly string CloneTooglePropName = ReflectionUtility.GetPropertyName((GitRepoModel s) => s.CloneToggle);
        
        public static readonly string UrlPropName = ReflectionUtility.GetPropertyName((GitRepoModel s) => s.Url);
        
        public static readonly string UsernamePropName = ReflectionUtility.GetPropertyName((GitRepoModel s) => s.Username);
        
        public static readonly string PasswordPropName = ReflectionUtility.GetPropertyName((GitRepoModel s) => s.Password);
        
        public static readonly string UseOverrideCredentialsPropName = ReflectionUtility.GetPropertyName((GitRepoModel s) => s.UseOverrideCredentials);
        
        public static readonly string CloneStatusPropName = ReflectionUtility.GetPropertyName((GitRepoModel s) => s.CloneStatus);
        
        public static readonly string CloneStatusMessagePropName = ReflectionUtility.GetPropertyName((GitRepoModel s) => s.CloneStatusMessage);
        //\Properties names

        //Bindable properties
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
                OnPropertyChanged(UseCustomRepoNamePropName);
                OnPropertyChanged(UseDefaultRepoNamePropName);
                OnPropertyChanged(RepoNamePropName);
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
                OnPropertyChanged(RepoNamePropName);
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
                OnPropertyChanged(UpdateTooglePropName);
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
                OnPropertyChanged(CloneTooglePropName);
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
                OnPropertyChanged(UrlPropName);
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
                OnPropertyChanged(UsernamePropName);
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
                OnPropertyChanged(PasswordPropName);
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
                OnPropertyChanged(UseOverrideCredentialsPropName);
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
                OnPropertyChanged(CloneStatusPropName);
                OnPropertyChanged(CloneStatusMessagePropName);
            }
        }
        //\Bindable properties

        //Implementation
        public GitRepoModel()
        {
            hash = Guid.NewGuid().ToString();
            useOverrideCredentials = false;
            username = string.Empty;
            cloneToogle = false;
            url = string.Empty;
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
        //\Implementation
    }
}
