using NugetWorkflow.UI.WpfUI.Common.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.UI.WpfUI.Common.Exceptions
{
    public class MissingViewException : BaseException
    {
        public MissingViewException(string message) : base(message)
        {

        }
    }
}
