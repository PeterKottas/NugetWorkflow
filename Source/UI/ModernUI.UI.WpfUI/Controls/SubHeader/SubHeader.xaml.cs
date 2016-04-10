using FirstFloor.ModernUI.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
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

namespace NugetWorkflow.UI.WpfUI.Controls
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class SubHeader : UserControl
    {
        /// <summary>
        /// Gets or sets the Label which is displayed next to the field
        /// </summary>
        public String Header
        {
            get { return (String)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Identified the Label dependency property
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string),
              typeof(SubHeader), new PropertyMetadata(""));

        public SubHeader()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
