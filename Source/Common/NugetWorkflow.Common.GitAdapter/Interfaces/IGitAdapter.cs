using NugetWorkflow.Common.GitAdapter.DTOs.Requests;
using NugetWorkflow.Common.GitAdapter.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.Common.GitAdapter.Interfaces
{
    public interface IGitAdapter
    {
        void CloneProjects(CloneProjectsRequestDTO request);

        void UpdateProjectsDependencies(UpdateProjectsDependenciesRequestDTO request);

        RepoBranchesResponseDTO GetRepoBranches(RepoBranchesRequestDTO request);
    }
}
