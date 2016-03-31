using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Shared.Enums
{
    public enum UpdateStatusEnum
    {
        CloneWrong,
        NuGetIDNotSpecified,
        NuGetVersionNotSpecified,
        NuGetVersionWrongFormat,
        NotContainNuGetID,
        Ok
    }
}
