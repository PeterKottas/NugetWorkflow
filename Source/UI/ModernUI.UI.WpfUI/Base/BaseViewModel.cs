﻿using FirstFloor.ModernUI.Presentation;
using NugetWorkflow.UI.WpfUI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.UI.WpfUI.Base
{
    public abstract class BaseViewModel : NotifyPropertyChanged
    {
        protected override void OnPropertyChanged(string propertyName)
        {
            SceneSaver.MakeDirty(this, propertyName);
            base.OnPropertyChanged(propertyName);
        }

        public void OnPropertyChangedExternal(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        protected void OnUndoRedoPropertyChanged(string propertyName, Action undo, Action redo)
        {
            UndoManager.RecordState(this, propertyName, undo, redo);
            OnPropertyChanged(propertyName);
        }
    }
}
