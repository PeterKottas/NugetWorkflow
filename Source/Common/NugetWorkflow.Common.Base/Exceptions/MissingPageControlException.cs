using NugetWorkflow.Common.Base.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.Common.Base.Exceptions
{
    [Serializable]
    public class MissingViewException : BaseException
    {
        public MissingViewException(string message) : base(message)
        {

        }
    }
}
