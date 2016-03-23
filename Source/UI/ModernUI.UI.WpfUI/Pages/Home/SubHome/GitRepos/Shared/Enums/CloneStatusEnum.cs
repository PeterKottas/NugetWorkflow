using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Shared.Enums
{
    public enum CloneStatusEnum
    {
        WrongUrlFormat,
        BasePathUndefined,
        BasePathWrongFormat,
        AlreadyExists,
        OK
    }
}
