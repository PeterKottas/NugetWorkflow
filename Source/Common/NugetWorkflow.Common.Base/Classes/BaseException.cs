using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.Common.Base.BaseClasses
{
    public abstract class BaseException : Exception
    {
        public BaseException()
            : base()
        {

        }

        public BaseException(string message)
            : base(message)
        {

        }

        public BaseException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
