using NugetWorkflow.Common.FilePatcher.DTOs.Requests;
using NugetWorkflow.Common.FilePatcher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.Plugins.FilePatcher
{
    public class FilePatcherCore : IFilePatcher
    {
        public void UpdateDependenciesVersion(UpdateDependenciesVersionRequestDTO request)
        {
            throw new NotImplementedException();
        }
    }
}
