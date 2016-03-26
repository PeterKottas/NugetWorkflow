using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

namespace NugetWorkflow.Common.GitAdapter.DTOs.Data
{
    public class GitRepoDTO
    {
        public string URL { get; set; }
        public string Username { get; set; }
        public SecureString Password { get; set; }

        public GitRepoDTO()
        {

        }
    }
}
