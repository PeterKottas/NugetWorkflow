using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NugetWorkflow.UI.WpfUI.Pages
{
    public class GitRepoViewModelDTO
    {
        public string Url { get; set; }
        public int Username { get; set; }
        public int Password { get; set; }
        public GitRepoViewModelDTO()
        {

        }
    }
}
