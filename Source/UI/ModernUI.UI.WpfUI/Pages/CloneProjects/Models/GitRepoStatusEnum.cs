﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.UI.WpfUI.Pages.CloneProjects.Models
{
    public enum GitRepoStatusEnum
    {
        WrongFormat,
        BasePathUndefined,
        AlreadyExists
    }
}
