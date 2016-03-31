using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Shared.Enums;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Extensions
{
    public static class UpdateStatusEnumExtensions
    {
        public static string ToUserFriendlyMessage(this UpdateStatusEnum source)
        {
            switch (source)
            {
                case UpdateStatusEnum.CloneWrong:
                    return "Not cloned";
                case UpdateStatusEnum.NotContainNuGetID:
                    return "Doesn't contain NuGet ID";
                case UpdateStatusEnum.Ok:
                    return "Ok";
                case UpdateStatusEnum.NuGetIDNotSpecified:
                    return "ID not specified";
                case UpdateStatusEnum.NuGetVersionNotSpecified:
                    return "Version not specified";
                case UpdateStatusEnum.NuGetVersionWrongFormat:
                    return "Version wrong format";
                default:
                    return "Unknown";
            }
        }
    }
}
