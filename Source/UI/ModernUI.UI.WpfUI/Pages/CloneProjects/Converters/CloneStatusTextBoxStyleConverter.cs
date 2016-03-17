using NugetWorkflow.UI.WpfUI.Pages.CloneProjects.Extensions;
using NugetWorkflow.UI.WpfUI.Pages.CloneProjects.Models;
using NugetWorkflow.UI.WpfUI.Pages.CLoneProjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace NugetWorkflow.UI.WpfUI.Pages.CloneProjects.Converters
{
    public class CloneStatusTextBoxStyleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var repoUrl = values[0] as string;
            var basePath = values[1] as string;
            var styleOk = values[2] as SolidColorBrush;
            var styleWarnning = values[3] as SolidColorBrush;

            if(string.IsNullOrEmpty(basePath))
            {
                return styleWarnning;
            }

            Uri uriResult;
            bool result = Uri.TryCreate(repoUrl, UriKind.Absolute, out uriResult);
            if (result)
            {
                var matchGroups = Regex.Match(repoUrl, @"([^/]+)\.git").Groups;
                if (matchGroups != null && matchGroups.Count > 0)
                {
                    var repoName = matchGroups[matchGroups.Count - 1].Value;
                    if(string.IsNullOrEmpty(repoName))
                    {
                        return styleWarnning;
                    }
                    if (!Directory.Exists(Path.Combine(basePath, repoName)))
                    {
                        return styleOk;
                    }
                    return styleWarnning;
                }
                else
                {
                    return styleWarnning;
                }
            }
            else
            {
                return styleWarnning;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
