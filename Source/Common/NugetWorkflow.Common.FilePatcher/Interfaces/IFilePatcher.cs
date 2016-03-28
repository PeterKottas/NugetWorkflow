using NugetWorkflow.Common.FilePatcher.DTOs.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.Common.FilePatcher.Interfaces
{
    public interface IFilePatcher
    {
        void UpdateDependenciesVersion(UpdateDependenciesVersionRequestDTO request);
    }
}
