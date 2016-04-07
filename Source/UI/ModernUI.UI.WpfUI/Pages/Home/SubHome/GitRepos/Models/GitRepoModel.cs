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
using NugetWorkflow.Common.GitAdapter.Interfaces;
using NugetWorkflow.Plugins.GitAdapter;
using NugetWorkflow.Common.GitAdapter.DTOs.Requests;
using NugetWorkflow.UI.WpfUI.Attributes;
using NugetWorkflow.UI.WpfUI.Base;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Models
{
    [SaveSceneAttribute]
    public class GitRepoModel : BaseViewModel
    {
        //Dependency interfaces
        IGitAdapter gitAdapter;
        //\Dependency interfaces

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

        [SaveSceneAttribute]
        private string repoName = null;

        [SaveSceneAttribute]
        private string repoNameCustom = null;

        private bool useCustomRepoName = false;

        private List<string> nuGetVersions;

        private List<string> repoBranches;

        private string updateBranch = "master";

        private bool useUpdateBranch = false;
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

        public static readonly string RepoBranchesPropName = ReflectionUtility.GetPropertyName((GitRepoModel s) => s.RepoBranches);

        public static readonly string UpdateBranchPropName = ReflectionUtility.GetPropertyName((GitRepoModel s) => s.UpdateBranch);

        public static readonly string UseUpdateBranchPropName = ReflectionUtility.GetPropertyName((GitRepoModel s) => s.UseUpdateBranch);
        //\Properties names

        //Bindable properties
        [SaveSceneAttribute]
        public bool UseUpdateBranch
        {
            get
            {
                return useUpdateBranch;
            }
            set
            {
                var orig = useUpdateBranch;
                useUpdateBranch = value;
                OnUndoRedoPropertyChanged(UseUpdateBranchPropName, () => useUpdateBranch = orig, () => useUpdateBranch = value);
            }
        }

        [SaveSceneAttribute]
        public string UpdateBranch
        {
            get
            {
                return updateBranch;
            }
            set
            {
                var orig = updateBranch;
                updateBranch = value;
                OnUndoRedoPropertyChanged(UpdateBranchPropName, () => updateBranch = orig, () => updateBranch = value);
            }
        }

        public List<string> RepoBranches
        {
            get
            {
                return repoBranches;
            }
            set
            {
                repoBranches = value;
                OnPropertyChanged(RepoBranchesPropName);
            }
        }
        
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

        [SaveSceneAttribute]
        public bool UseCustomRepoName
        {
            get
            {
                return useCustomRepoName;
            }
            set
            {
                var orig = useCustomRepoName;
                useCustomRepoName = value;
                OnUndoRedoPropertyChanged(UseCustomRepoNamePropName, () => useCustomRepoName = orig, () => useCustomRepoName = value);

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
                UpdateRepoBranches();
            }
        }

        [SaveSceneAttribute]
        public bool UpdateToggle
        {
            get
            {
                return updateToogle;
            }
            set
            {
                var orig = updateToogle;
                updateToogle = value;
                OnUndoRedoPropertyChanged(UpdateTooglePropName, () => updateToogle = orig, () => updateToogle = value);
            }
        }

        [SaveSceneAttribute]
        public bool CloneToggle
        {
            get
            {
                return cloneToogle;
            }
            set
            {
                var orig = cloneToogle;
                cloneToogle = value;
                OnUndoRedoPropertyChanged(CloneTooglePropName, () => cloneToogle = orig, () => cloneToogle = value);
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

        [SaveSceneAttribute]
        public string Url
        {
            get
            {
                return url;
            }
            set
            {
                var orig = url;
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
                OnUndoRedoPropertyChanged(UrlPropName, () => url = orig, () => url = value);
            }
        }

        [SaveSceneAttribute]
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                var orig = username;
                username = value;
                OnUndoRedoPropertyChanged(UsernamePropName, () => username = orig, () => username = value);
            }
        }

        [SaveSceneAttribute]
        public SecureString Password
        {
            get
            {
                return password;
            }
            set
            {
                var orig = password;
                password = value;
                OnUndoRedoPropertyChanged(PasswordPropName, () => password = orig, () => password = value);
            }
        }

        [SaveSceneAttribute]
        public bool UseOverrideCredentials
        {
            get
            {
                return useOverrideCredentials;
            }
            set
            {
                var orig = useOverrideCredentials;
                useOverrideCredentials = value;
                OnUndoRedoPropertyChanged(UseOverrideCredentialsPropName, () => useOverrideCredentials = orig, () => useOverrideCredentials = value);
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
            username = null;
            cloneToogle = false;
            url = null;
            gitAdapter = new GitAdapterCore();
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

        public void UpdateRepoBranches(string basePath)
        {
            RepoBranches = GetRepoBranches(basePath);
        }

        public void UpdateRepoBranches()
        {
            var basePath = ViewModelService.GetViewModel<BaseSetupViewModel>().BasePath;
            UpdateRepoBranches(basePath);
        }

        private List<string> GetRepoBranches(string basePath)
        {
            if (!string.IsNullOrEmpty(basePath) && !string.IsNullOrEmpty(RepoName))
            {
                return gitAdapter.GetRepoBranches(new RepoBranchesRequestDTO()
                    {
                        Path = Path.Combine(basePath, repoName)
                    }).RepoBranches;
            }
            else
            {
                return new List<string>();
            }
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
            if (!string.IsNullOrEmpty(basePath) && !string.IsNullOrEmpty(RepoName))
            {
                var path = Path.Combine(basePath, RepoName);
                if (Directory.Exists(path))
                {
                    var files = Directory.GetFiles(path, "packages.config", SearchOption.AllDirectories);
                    var nuGetIDs = new List<string>();
                    foreach (var file in files)
                    {
                        var doc = new XmlDocument();
                        try
                        {
                            doc.Load(file);
                        }
                        catch (Exception)
                        {
                            return new List<string>();
                        }
                        var elements = doc.GetElementsByTagName("package");
                        foreach (XmlNode element in elements)
                        {
                            nuGetIDs.Add(element.Attributes["id"].Value);
                        }
                    }
                    return nuGetIDs.Distinct().Where(a => a != null).ToList();
                }
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
