using FirstFloor.ModernUI.Windows.Controls;
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

namespace NugetWorkflow.UI.WpfUI.Pages.Home
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class HomePage : UserControl
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void navigateToPage(string PageName)
        {
            try
            {
                BBCodeBlock bs = new BBCodeBlock();
                bs.LinkNavigator.Navigate(new Uri(PageName, UriKind.Relative), this);
            }
            catch (Exception error)
            {
                ModernDialog.ShowMessage(error.Message, FirstFloor.ModernUI.Resources.NavigationFailed, MessageBoxButton.OK);
            }
        }

        private void cloneButtonClick(object sender, RoutedEventArgs e)
        {
            navigateToPage("/Pages/CloneProjects/CloneProjectsPage.xaml");
        }

        private void updatePackageClick(object sender, RoutedEventArgs e)
        {
            navigateToPage("/Pages/UpdatePackage/UpdatePackagePage.xaml");
        }
    }
}
