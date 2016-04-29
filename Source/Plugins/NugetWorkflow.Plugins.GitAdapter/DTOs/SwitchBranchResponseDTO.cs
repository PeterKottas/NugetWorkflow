using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.Plugins.GitAdapter.DTOs
{
    public class SwitchBranchResponseDTO
    {
        public string InitialBranchName { get; set; }

        public bool Switched { get; set; }

        public bool Stashed { get; set; }

        public StashDTO StashResponse { get; set; }
    }
}
