using FirstFloor.ModernUI.Presentation;
using NugetWorkflow.UI.WpfUI.Base;
using NugetWorkflow.UI.WpfUI.Pages.Home;
using NugetWorkflow.UI.WpfUI.Pages.Settings.SubSettings.Analytics;
using NugetWorkflow.UI.WpfUI.Utils.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.UI.WpfUI.Utils
{
    public static class UndoManager
    {
        private static List<UndoStatesDTO> undoBuffer = new List<UndoStatesDTO>();
        private static int undoBufferIndex = 0;
        private static bool enabled = true;
        private static int undoRedoLimit = 1000;

        public static bool IsEnabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
                ViewModelService.GetViewModel<HomePageViewModel>().UndoCommand.OnCanExecuteChanged();
                ViewModelService.GetViewModel<HomePageViewModel>().RedoCommand.OnCanExecuteChanged();
            }
        }

        public static int CurrentBufferSize
        {
            get
            {
                return undoBuffer.Count;
            }
        }

        public static int CurrentBufferIndex
        {
            get
            {
                return undoBufferIndex;
            }
        }

        public static void ResetBuffer()
        {
            undoBuffer.Clear();
            undoBufferIndex = 0;
        }

        public static void Enable()
        {
            IsEnabled = true;
        }

        public static void Disable()
        {
            IsEnabled = false;
        }

        public static int UndoRedoLimit
        {
            get
            {
                return undoRedoLimit;
            }
            set
            {
                undoRedoLimit = value;
            }
        }

        private static void ProcessAnalytics()
        {
            ViewModelService.GetViewModel<AnalyticsViewModel>().OnPropertyChangedExternal(AnalyticsViewModel.UndoBufferSizePropName);
            ViewModelService.GetViewModel<AnalyticsViewModel>().OnPropertyChangedExternal(AnalyticsViewModel.UndoBufferIndexPropName);
        }

        public static void RecordState(object container, string propetyName, Action undo, Action redo)
        {
            if (enabled)
            {
                if (undoBufferIndex > 0)
                {
                    undoBuffer.RemoveRange(0, undoBufferIndex);
                    undoBufferIndex = 0;
                    ViewModelService.GetViewModel<AnalyticsViewModel>().OnPropertyChangedExternal(AnalyticsViewModel.UndoBufferIndexPropName);
                }
                undoBuffer.Insert(0, new UndoStatesDTO()
                    {
                        Container = container,
                        PropertyName = propetyName,
                        Undo = undo,
                        Redo = redo
                    });
                enabled = false;
                redo();
                enabled = true;
                ViewModelService.GetViewModel<AnalyticsViewModel>().OnPropertyChangedExternal(AnalyticsViewModel.UndoBufferSizePropName);
                ViewModelService.GetViewModel<HomePageViewModel>().UndoCommand.OnCanExecuteChanged();
                ViewModelService.GetViewModel<HomePageViewModel>().RedoCommand.OnCanExecuteChanged();
            }
            else
            {
                redo();
            }
        }

        public static void Undo()
        {
            if (undoBufferIndex < undoBuffer.Count && undoBufferIndex >= 0 && enabled)
            {
                try
                {
                    var dto = undoBuffer[undoBufferIndex];
                    enabled = false;
                    dto.Undo();
                    enabled = true;
                    (dto.Container as BaseViewModel).OnPropertyChangedExternal(dto.PropertyName);
                    ViewModelService.GetViewModel<HomePageViewModel>().UndoCommand.OnCanExecuteChanged();
                    ViewModelService.GetViewModel<HomePageViewModel>().RedoCommand.OnCanExecuteChanged();
                    undoBufferIndex++;
                    ViewModelService.GetViewModel<AnalyticsViewModel>().OnPropertyChangedExternal(AnalyticsViewModel.UndoBufferIndexPropName);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public static bool UndoCanExecute()
        {
            if (undoBufferIndex < undoBuffer.Count && undoBufferIndex >= 0 && enabled)
            {
                return true;
            }
            return false;
        }

        public static void Redo()
        {
            if (undoBufferIndex > 0 && undoBufferIndex <= undoBuffer.Count && enabled)
            {
                try
                {
                    undoBufferIndex--;
                    var dto = undoBuffer[undoBufferIndex];
                    enabled = false;
                    dto.Redo();
                    enabled = true;
                    (dto.Container as BaseViewModel).OnPropertyChangedExternal(dto.PropertyName);
                    ViewModelService.GetViewModel<HomePageViewModel>().UndoCommand.OnCanExecuteChanged();
                    ViewModelService.GetViewModel<HomePageViewModel>().RedoCommand.OnCanExecuteChanged();
                    ViewModelService.GetViewModel<AnalyticsViewModel>().OnPropertyChangedExternal(AnalyticsViewModel.UndoBufferIndexPropName);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public static bool RedoCanExecute()
        {
            if (undoBufferIndex > 0 && undoBufferIndex <= undoBuffer.Count && enabled)
            {
                return true;
            }
            return false;
        }
    }
}
