using NugetWorkflow.Common.GitAdapter.DTOs.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.Common.GitAdapter.DTOs.Requests
{
    public class CloneProjectsRequestDTO
    {
        public string BasePath { get; set; }
        public List<GitRepoDTO> ListOfRepos { get; set; }
    }
}
