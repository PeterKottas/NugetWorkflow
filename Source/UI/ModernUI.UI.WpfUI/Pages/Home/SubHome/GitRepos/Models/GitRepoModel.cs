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
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.Update;
using System.Xml;

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

        private SetupStatusEnum setupStatus;

        private CloneStatusEnum cloneStatus;

        private UpdateStatusEnum updateStatus;

        private bool isValidUrl = false;

        private string repoName = null;

        private string repoNameCustom = null;

        private bool useCustomRepoName = false;

        private List<string> nuGetVersions;
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

        public static readonly string UpdateStatusPropName = ReflectionUtility.GetPropertyName((GitRepoModel s) => s.UpdateStatus);

        public static readonly string UpdateStatusMessagePropName = ReflectionUtility.GetPropertyName((GitRepoModel s) => s.UpdateStatusMessage);

        public static readonly string SetupStatusPropName = ReflectionUtility.GetPropertyName((GitRepoModel s) => s.SetupStatus);

        public static readonly string SetupStatusMessagePropName = ReflectionUtility.GetPropertyName((GitRepoModel s) => s.SetupStatusMessage);

        public static readonly string NuGetVersionsPropName = ReflectionUtility.GetPropertyName((GitRepoModel s) => s.NuGetVersions);
        //\Properties names

        //Bindable properties
        public List<string> NuGetVersions
        {
            get
            {
                return nuGetVersions;
            }
            set
            {
                nuGetVersions = value;
                OnPropertyChanged(NuGetVersionsPropName);
            }
        }

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
                UpdateSetupStatus();
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
                    UpdateSetupStatus();
                }
                else
                {
                    repoName = value;
                }
                OnPropertyChanged(RepoNamePropName);
                UpdatePackagesIDs();
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
                UpdateSetupStatus();
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

        public SetupStatusEnum SetupStatus
        {
            get
            {
                return setupStatus;
            }
            set
            {
                setupStatus = value;
                OnPropertyChanged(SetupStatusPropName);
                OnPropertyChanged(SetupStatusMessagePropName);
            }
        }

        public string SetupStatusMessage
        {
            get
            {
                return SetupStatus.ToUserFriendlyMessage();
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

        public string CloneStatusMessage
        {
            get
            {
                return CloneStatus.ToUserFriendlyMessage();
            }
        }

        public UpdateStatusEnum UpdateStatus
        {
            get
            {
                return updateStatus;
            }
            set
            {
                updateStatus = value;
                OnPropertyChanged(UpdateStatusPropName);
                OnPropertyChanged(UpdateStatusMessagePropName);
            }
        }

        public string UpdateStatusMessage
        {
            get
            {
                return UpdateStatus.ToUserFriendlyMessage();
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

        public void UpdateSetupStatus()
        {
            var basePath = ViewModelService.GetViewModel<BaseSetupViewModel>().BasePath;
            UpdateSetupStatus(basePath);
        }

        public void UpdateSetupStatus(string basePath)
        {
            SetupStatus = GetSetupStatus(basePath);
            UpdateCloneStatus(basePath);
            UpdateUpdateStatus();
        }

        public void UpdatePackagesIDs()
        {
            var basePath = ViewModelService.GetViewModel<BaseSetupViewModel>().BasePath;
            UpdatePackagesIDs(basePath);
        }

        public void UpdatePackagesIDs(string basePath)
        {
            NuGetVersions = GetPackagesIDs(basePath);
        }

        private List<string> GetPackagesIDs(string basePath)
        {
            var path = Path.Combine(basePath, RepoName);
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path, "packages.config", SearchOption.AllDirectories);
                var nuGetIDs = new List<string>();
                foreach (var file in files)
                {
                    var doc = new XmlDocument();
                    doc.Load(file);
                    var elements = doc.GetElementsByTagName("package");
                    foreach (XmlNode element in elements)
                    {
                        nuGetIDs.Add(element.Attributes["id"].Value);
                    }
                }
                return nuGetIDs.Distinct().Where(a => a != null).ToList();
            }
            return new List<string>();
        }

        private SetupStatusEnum GetSetupStatus(string basePath)
        {
            if (string.IsNullOrEmpty(basePath))
            {
                return SetupStatusEnum.BasePathUndefined;
            }
            try
            {
                Path.GetFullPath(basePath);
            }
            catch (Exception)
            {
                return SetupStatusEnum.BasePathWrongFormat;
            }
            if (string.IsNullOrEmpty(Url))
            {
                return SetupStatusEnum.UrlNotDefined;
            }
            if (string.IsNullOrEmpty(RepoName))
            {
                return SetupStatusEnum.UrlWrongFormat;
            }
            if (!Directory.Exists(basePath))
            {
                return SetupStatusEnum.BasePathNotFound;
            }
            return SetupStatusEnum.OK;
        }

        public void UpdateCloneStatus(string basePath)
        {
            CloneStatus = GetCloneStatus(basePath);
        }

        public void UpdateCloneStatus()
        {
            var basePath = ViewModelService.GetViewModel<BaseSetupViewModel>().BasePath;
            UpdateCloneStatus(basePath);
        }

        private CloneStatusEnum GetCloneStatus(string basePath)
        {
            if (SetupStatus != SetupStatusEnum.OK)
            {
                return CloneStatusEnum.SetupWrong;
            }
            if (Directory.Exists(Path.Combine(basePath, RepoName)))
            {
                return CloneStatusEnum.AlreadyCloned;
            }
            return CloneStatusEnum.CanBeCloned;
        }

        public void UpdateUpdateStatus(string basePath, string nuGetID, string nuGetVersion)
        {
            UpdateStatus = GetUpdateStatus(basePath, nuGetID, nuGetVersion);
        }

        public void UpdateUpdateStatus()
        {
            var basePath = ViewModelService.GetViewModel<BaseSetupViewModel>().BasePath;
            var nuGetID = ViewModelService.GetViewModel<UpdateViewModel>().NuGetID;
            var nuGetVersion = ViewModelService.GetViewModel<UpdateViewModel>().NuGetVersion;
            UpdateUpdateStatus(basePath, nuGetID, nuGetVersion);
        }

        private UpdateStatusEnum GetUpdateStatus(string basePath, string nuGetID, string nuGetVersion)
        {
            if (CloneStatus != CloneStatusEnum.AlreadyCloned)
            {
                return UpdateStatusEnum.CloneWrong;
            }
            if (string.IsNullOrEmpty(nuGetID))
            {
                return UpdateStatusEnum.NuGetIDNotSpecified;
            }
            if (string.IsNullOrEmpty(nuGetVersion))
            {
                return UpdateStatusEnum.NuGetVersionNotSpecified;
            }
            if (!Regex.IsMatch(nuGetVersion, @"^(?:[\[\(]?\,?)(?:(\d+)\.)?(?:(\d+)\.)?(?:(\*|\d+)(\-?\w*)\,?)(?:(\d+)\.)?(?:(\d+)\.)?(\*|\d+)?(\-?\w*)?(?:[\]\)]?)$", RegexOptions.None, new TimeSpan(100)))
            {
                return UpdateStatusEnum.NuGetVersionWrongFormat;
            }
            return UpdateStatusEnum.Ok;
        }
        //\Implementation
    }
}
