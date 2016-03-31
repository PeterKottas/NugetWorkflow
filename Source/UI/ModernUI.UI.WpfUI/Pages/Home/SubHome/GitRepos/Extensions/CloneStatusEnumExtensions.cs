using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Shared.Enums;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Extensions
{
    public static class CloneStatusEnumExtensions
    {
        public static string ToUserFriendlyMessage(this CloneStatusEnum source)
        {
            switch (source)
            {
                case CloneStatusEnum.AlreadyCloned:
                    return "Already cloned";
                case CloneStatusEnum.CanBeCloned:
                    return "Can be cloned";
                case CloneStatusEnum.SetupWrong:
                    return "Repo setup wrong";
                default:
                    return "Unknown";
            }
        }
    }
}
