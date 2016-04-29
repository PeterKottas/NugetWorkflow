using NugetWorkflow.Common.Base.DTOs.GitRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.Common.FilePatcher.DTOs.Requests
{
    public class UpdateDependenciesVersionRequestDTO
    {
        public string GitReposPath { get; set; }

        public string NuGetID { get; set; }

        public string NewVersion { get; set; }
    }
}
