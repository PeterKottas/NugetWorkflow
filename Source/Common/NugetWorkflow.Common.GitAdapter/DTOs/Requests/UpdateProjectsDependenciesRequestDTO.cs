using NugetWorkflow.Common.Base.DTOs.GitRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.Common.GitAdapter.DTOs.Requests
{
    public class UpdateProjectsDependenciesRequestDTO
    {
        private Action<bool, string> progressAction;

        private Action<string> finishedAction;

        public string NugetID { get; set; }

        public string Version { get; set; }

        public List<GitRepoDTO> ListOfRepos { get; set; }

        public Action<bool, string> ProgressAction
        {
            get
            {
                if (progressAction == null)
                {
                    return (e, b) => { };
                }
                return progressAction;
            }
            set
            {
                progressAction = value;
            }
        }

        public Action<string> FinishedAction
        {
            get
            {
                if (finishedAction == null)
                {
                    return (e) => { };
                }
                return finishedAction;
            }
            set
            {
                finishedAction = value;
            }
        }
    }
}
