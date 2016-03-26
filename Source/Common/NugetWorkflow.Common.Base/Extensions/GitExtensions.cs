using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NugetWorkflow.Common.Base.Extensions
{
    public static class GitExtensions
    {
        public static bool GetFolderFromUrl(this string url, ref string FolderName)
        {
            var matchGroups = Regex.Match(url, @"([^/]+)\.git").Groups;
            if (matchGroups != null && matchGroups.Count > 0)
            {
                FolderName = matchGroups[matchGroups.Count - 1].Value;
                return true;
            }
            return false;
        }
    }
}
