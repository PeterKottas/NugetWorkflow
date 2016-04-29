using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.Common.Base.Utils;
using NugetWorkflow.UI.WpfUI.Attributes;
using NugetWorkflow.UI.WpfUI.Base;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.Update;
using NugetWorkflow.UI.WpfUI.Pages.Settings.SubSettings.General;
using NugetWorkflow.UI.WpfUI.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.UI.WpfUI.Pages.Settings.SubSettings.Analytics
{
    [SaveConfigAttribute]
    public class AnalyticsViewModel : BaseViewModel, IViewModel
    {
        //Private properties

        //\Private properties

        //Data hiding

        //\Data hiding

        //Properties names        
        public static readonly string UndoBufferSizePropName = ReflectionUtility.GetPropertyName((AnalyticsViewModel s) => s.UndoBufferSize);
        public static readonly string UndoBufferIndexPropName = ReflectionUtility.GetPropertyName((AnalyticsViewModel s) => s.UndoBufferIndex);
        //\Properties names        

        //Bindable properties
        public GeneralSettingsViewModel GeneralSettingsVM
        {
            get
            {
                return ViewModelService.GetViewModel<GeneralSettingsViewModel>();
            }
        }

        public int UndoBufferSize
        {
            get
            {
                return UndoManager.CurrentBufferSize;
            }
        }

        public int UndoBufferIndex
        {
            get
            {
                return UndoManager.CurrentBufferIndex;
            }
        }
        //\Bindable properties

        //Implementation
        public AnalyticsViewModel()
        {
            
        }
        //\Implementation

        public void Initialize()
        {
        }
    }
}
