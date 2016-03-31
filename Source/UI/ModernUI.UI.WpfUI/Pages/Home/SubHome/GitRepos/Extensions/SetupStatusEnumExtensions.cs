using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Shared.Enums;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Extensions
{
    public static class SetupStatusEnumExtensions
    {
        public static string ToUserFriendlyMessage(this SetupStatusEnum source)
        {
            switch (source)
            {
                case SetupStatusEnum.UrlWrongFormat:
                    return "Wrong url format";
                case SetupStatusEnum.UrlNotDefined:
                    return "Url not defined";
                case SetupStatusEnum.BasePathUndefined:
                    return "Base path missing";
                case SetupStatusEnum.BasePathWrongFormat:
                    return "Base path incorrect";
                case SetupStatusEnum.OK:
                    return "Ok";
                case SetupStatusEnum.BasePathNotFound:
                    return "Base path not found";
                default:
                    return "Unknown";
            }
        }
    }
}
