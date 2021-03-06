﻿using FirstFloor.ModernUI.Windows.Controls;
using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.UI.WpfUI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NugetWorkflow.UI.WpfUI.Pages.Settings.SubSettings.Update
{
    /// <summary>
    /// Interaction logic for General.xaml
    /// </summary>
    public partial class UpdatePage : UserControl, IPageUserControl
    {
        public UpdatePage()
        {
            InitializeComponent();
            AssignViewModel();
            AddUserControl();
        }

        public void AssignViewModel()
        {
            this.DataContext = ViewModelService.GetViewModel<UpdateViewModel>();
        }

        public void AddUserControl()
        {
            PageUserControlService.AddUserControl(this);
        }
    }
}
