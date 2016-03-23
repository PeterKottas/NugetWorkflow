using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Converters
{
    public class CredentialsOverrideConverter : DependencyObject, IMultiValueConverter
    {
        public bool UseOverride
        {
            get 
            { 
                return (bool)GetValue(UseOverrideProperty); 
            }
            set 
            { 
                SetValue(UseOverrideProperty, value); 
            }
        }

        // The dependency property to allow the property to be used from XAML.
        public static readonly DependencyProperty UseOverrideProperty =
            DependencyProperty.Register(
            "UseOverride",
            typeof(bool),
            typeof(CredentialsOverrideConverter));

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var overridenUsername = values[0] as string;
            var repoUsername = values[1] as string;
            var overrideUsername = (bool)values[2];
            if (overrideUsername)
            {
                return overridenUsername;
            }
            return repoUsername;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new[] { Binding.DoNothing, value, Binding.DoNothing };
        }
    }
}
