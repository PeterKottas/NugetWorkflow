﻿using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Shared.Enums;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Extensions
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
                case CloneStatusEnum.BasePathWrongFormat:
                    return "Base path incorrect";
                default:
                    return "Unknown";
            }
        }
    }
}
