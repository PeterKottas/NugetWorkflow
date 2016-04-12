using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.Common.Base.Utils;
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
    public class UpdateViewModel : BaseViewModel, IViewModel
    {
        //Private properties

        //\Private properties

        //Data hiding
        private bool automaticUpdatesEnabled;
        private string updateMessage;
        //\Data hiding

        //Properties names        
        private static readonly string UpdateMessagePropName = ReflectionUtility.GetPropertyName((UpdateViewModel s) => s.UpdateMessage);
        //\Properties names     

        //Bindable properties
        public string CurrentVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public bool AutomaticUpdatesEnabled
        {
            get
            {
                return automaticUpdatesEnabled;
            }
            set
            {
                automaticUpdatesEnabled = value;
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
        //\Bindable properties

        public UpdateViewModel()
        {
            Initialize();
            CheckForUpdates();
        }

        public void Initialize()
        {
        }

        async public void CheckForUpdates()
        {
            try
            {
                using (var mgr = new UpdateManager(@"http://ec2-52-16-197-231.eu-west-1.compute.amazonaws.com:1234/release"))
                {
                    var result = await mgr.CheckForUpdate();
                    if (result.ReleasesToApply.Count() > 0 && AutomaticUpdatesEnabled)
                    {
                         await mgr.UpdateApp();
                         updateMessage = string.Format("");
                    }
                }
            }
            catch (Exception exception)
            {
                updateMessage = exception.Message;
            }
        }
    }
}
