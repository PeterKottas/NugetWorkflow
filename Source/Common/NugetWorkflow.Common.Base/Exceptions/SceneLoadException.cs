using NugetWorkflow.Common.Base.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.Common.Base.Exceptions
{
    [Serializable]
    public class SceneLoadException : BaseException
    {
        public SceneLoadException(string message)
            : base(message)
        {

        }
        public SceneLoadException(string message, Exception e)
            : base(message, e)
        {

        }
    }
}
