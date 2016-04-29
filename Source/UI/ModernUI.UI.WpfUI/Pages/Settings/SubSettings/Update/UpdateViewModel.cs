using FirstFloor.ModernUI.Presentation;
using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.Common.Base.Utils;
using NugetWorkflow.UI.WpfUI.Attributes;
using NugetWorkflow.UI.WpfUI.Base;
using Squirrel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.UI.WpfUI.Pages.Settings.SubSettings.Update
{
    [SaveConfigAttribute]
    public class UpdateViewModel : BaseViewModel, IViewModel
    {
        //Constants
        private const string updateNow = "Update now";
        private const string updateNowProgress = "Updating";
        //\Constants

        //Private properties

        //\Private properties

        //Data hiding
        private bool automaticUpdatesEnabled;
        private string updateMessage;
        private bool updateNowEnabled = true;
        private string updateURL;
        private string updateNowButtonText = updateNow;
        //\Data hiding

        //Properties names        
        private static readonly string UpdateMessagePropName = ReflectionUtility.GetPropertyName((UpdateViewModel s) => s.UpdateMessage);
        private static readonly string UpdateURLPropName = ReflectionUtility.GetPropertyName((UpdateViewModel s) => s.UpdateURL);
        private static readonly string UpdateNowEnabledPropName = ReflectionUtility.GetPropertyName((UpdateViewModel s) => s.UpdateNowEnabled);
        private static readonly string UpdateNowButtonTextPropName = ReflectionUtility.GetPropertyName((UpdateViewModel s) => s.UpdateNowButtonText);
        private static readonly string AutomaticUpdatesEnabledPropName = ReflectionUtility.GetPropertyName((UpdateViewModel s) => s.AutomaticUpdatesEnabled);
        //\Properties names     

        //Bindable properties
        public string CurrentVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        [SaveConfigAttribute]
        public bool AutomaticUpdatesEnabled
        {
            get
            {
                return automaticUpdatesEnabled;
            }
            set
            {
                automaticUpdatesEnabled = value;
                OnPropertyChanged(AutomaticUpdatesEnabledPropName);
            }
        }

        public string UpdateMessage
        {
            get
            {
                return updateMessage;
            }
            set
            {
                updateMessage = value;
                OnPropertyChanged(UpdateMessagePropName);
            }
        }

        [SaveConfigAttribute]
        public string UpdateURL
        {
            get
            {
                return updateURL;
            }
            set
            {
                updateURL = value;
                OnPropertyChanged(UpdateURLPropName);
            }
        }

        public bool UpdateNowEnabled
        {
            get
            {
                return updateNowEnabled;
            }
            set
            {
                updateNowEnabled = value;
                OnPropertyChanged(UpdateNowEnabledPropName);
            }
        }

        public string UpdateNowButtonText
        {
            get
            {
                return updateNowButtonText;
            }
            set
            {
                updateNowButtonText = value;
                OnPropertyChanged(UpdateNowButtonTextPropName);
            }
        }

        public RelayCommand UpdateNowCommand { get; set; }
        public RelayCommand CheckUpdatesCommand { get; set; }
        //\Bindable properties 

        //Implementation
        public UpdateViewModel()
        {
            Initialize();
            UpdateNowCommand = new RelayCommand(UpdateNowExecute);
            CheckUpdatesCommand = new RelayCommand(CheckUpdatesExecute);
        }

        private void CheckUpdatesExecute(object obj)
        {
            CheckUI();
        }

        private void UpdateNowExecute(object obj)
        {
            UpdateUI(false);
        }

        public void Initialize()
        {
#if DEBUG
            updateURL = @"http://ec2-52-16-197-231.eu-west-1.compute.amazonaws.com:1234/release";
#endif
            //UpdateUI();
        }

        public void CheckUI()
        {
            new Task(() =>
            {
                UpdateNowEnabled = false;
                Check().Wait();
                UpdateNowEnabled = true;
            }).Start();
        }

        public void UpdateUI(bool automatic = true)
        {
            new Task(() =>
            {
                UpdateNowEnabled = false;
                UpdateNowButtonText = updateNowProgress;
                Update(automatic).Wait();
                UpdateNowButtonText = updateNow;
                UpdateNowEnabled = true;
            }).Start();
        }

        private async Task Check()
        {
            try
            {
                using (var mgr = new UpdateManager(UpdateURL))
                {
                    var result = await mgr.CheckForUpdate();
                    if (result.ReleasesToApply.Count() == 0)
                    {
                        UpdateMessage = "You have the latest version";
                    }
                    else
                    {
                        UpdateMessage = string.Format("Latest version {0}. Update by clicking \"{1}\"", result.FutureReleaseEntry.Version.ToString(), updateNow);
                    }
                }
            }
            catch (Exception exception)
            {
                updateMessage = string.Format("Exception [{0}] prevented us from updating your app", exception.Message);
            }
        }

        private async Task Update(bool automatic = true)
        {
            try
            {
                using (var mgr = new UpdateManager(UpdateURL))
                {
                    var result = await mgr.CheckForUpdate();
                    if (result.ReleasesToApply.Count() == 0)
                    {
                        UpdateMessage = "You have the latest version";
                    }
                    else if (AutomaticUpdatesEnabled || !automatic)
                    {
                        mgr.UpdateApp().Wait();
                        UpdateMessage = string.Format("Your app was successfully updated to version {0}", result.FutureReleaseEntry.Version.ToString());
                    }
                    else
                    {
                        UpdateMessage = string.Format("Latest version {0} will not be installed because automatic updates are disabled", result.FutureReleaseEntry.Version.ToString());
                    }
                }
            }
            catch (Exception exception)
            {
                updateMessage = string.Format("Exception [{0}] prevented us from updating your app", exception.Message);
            }
        }
        //\Implementation
    }
}
