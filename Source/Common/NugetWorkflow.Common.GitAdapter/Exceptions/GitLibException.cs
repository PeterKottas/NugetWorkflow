using NugetWorkflow.Common.Base.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.Common.GitAdapter.Exceptions
{
    [Serializable]
    public class GitLibException : BaseException
    {
        public GitLibException(string message)
            : base(message)
        {

        }
    }
}
