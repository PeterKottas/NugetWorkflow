using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Extensions;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Shared.Enums;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Converters
{
    public class CloneStatusConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string repoUrl = values[0] as string;
            string basePath = values[1] as string;

            if(string.IsNullOrEmpty(basePath))
            {
                return CloneStatusEnum.BasePathUndefined.ToUserFriendlyMessage();
            }
            try 
	        {
                Path.GetFullPath(basePath);
	        }
	        catch (Exception)
	        {
                return CloneStatusEnum.BasePathWrongFormat.ToUserFriendlyMessage();
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
                        return CloneStatusEnum.WrongUrlFormat.ToUserFriendlyMessage();
                    }
                    if (!Directory.Exists(Path.Combine(basePath, repoName)))
                    {
                        return CloneStatusEnum.OK.ToUserFriendlyMessage();
                    }
                    return CloneStatusEnum.AlreadyExists.ToUserFriendlyMessage();
                }
                else
                {
                    return CloneStatusEnum.WrongUrlFormat.ToUserFriendlyMessage();
                }
            }
            else
            {
                return CloneStatusEnum.WrongUrlFormat.ToUserFriendlyMessage();
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
