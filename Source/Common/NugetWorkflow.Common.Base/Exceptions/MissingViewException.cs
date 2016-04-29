using NugetWorkflow.Common.Base.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.Common.Base.Exceptions
{
    [Serializable]
    public class MissingPageControlException : BaseException
    {
        public MissingPageControlException(string message)
            : base(message)
        {

        }
    }
}
