using NugetWorkflow.Common.GitAdapter.DTOs.Requests;
using NugetWorkflow.Common.GitAdapter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.Plugins.GitAdapter
{
    public class GitAdapterCore : IGitAdapter
    {
        public void CloneProjects(CloneProjectsRequestDTO request)
        {
            throw new NotImplementedException();
        }
    }
}
