using NugetWorkflow.UI.WpfUI.Pages.CloneProjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.UI.WpfUI.Pages.CloneProjects.Extensions
{
    public static class CloneStatusEnumExtensions
    {
        public static string ToUserFriendlyMessage(this CloneStatusEnum source)
        {
            switch (source)
            {
                case CloneStatusEnum.WrongUrlFormat:
                    return "Wrong url format";
                case CloneStatusEnum.BasePathUndefined:
                    return "Base path missing";
                case CloneStatusEnum.AlreadyExists:
                    return "Already exists";
                case CloneStatusEnum.OK:
                    return "Ok";
                default:
                    return "Unknown";
            }
        }
    }
}
