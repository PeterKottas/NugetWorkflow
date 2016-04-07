﻿using FirstFloor.ModernUI.Presentation;
using NugetWorkflow.UI.WpfUI.Base;
using NugetWorkflow.UI.WpfUI.Pages.Home;
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

        public static void ResetBuffer()
        {
            undoBuffer.Clear();
            undoBufferIndex = 0;
        }

        public static void Enable()
        {
            enabled = true;
        }

        public static void Disable()
        {
            enabled = false;
        }

        public static void RecordState(object container, string propetyName, Action undo, Action redo)
        {
            if (enabled)
            {
                if (undoBufferIndex > 0)
                {
                    undoBuffer.RemoveRange(0, undoBufferIndex);
                    undoBufferIndex = 0;
                }
                undoBuffer.Insert(0, new UndoStatesDTO()
                    {
                        Container = container,
                        PropertyName = propetyName,
                        Undo = undo,
                        Redo = redo
                    });
                ViewModelService.GetViewModel<HomePageViewModel>().UndoCommand.OnCanExecuteChanged();
                ViewModelService.GetViewModel<HomePageViewModel>().RedoCommand.OnCanExecuteChanged();
            }
        }

        public static void Undo()
        {
            if (undoBufferIndex < undoBuffer.Count && undoBufferIndex >= 0 && enabled)
            {
                try
                {
                    var dto = undoBuffer[undoBufferIndex];
                    dto.Undo();
                    (dto.Container as BaseViewModel).OnPropertyChangedExternal(dto.PropertyName);
                    ViewModelService.GetViewModel<HomePageViewModel>().UndoCommand.OnCanExecuteChanged();
                    ViewModelService.GetViewModel<HomePageViewModel>().RedoCommand.OnCanExecuteChanged();
                    undoBufferIndex++;
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
                    dto.Redo();
                    (dto.Container as BaseViewModel).OnPropertyChangedExternal(dto.PropertyName);
                    ViewModelService.GetViewModel<HomePageViewModel>().UndoCommand.OnCanExecuteChanged();
                    ViewModelService.GetViewModel<HomePageViewModel>().RedoCommand.OnCanExecuteChanged();
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
