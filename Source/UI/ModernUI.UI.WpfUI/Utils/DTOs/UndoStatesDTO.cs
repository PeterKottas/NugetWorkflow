using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.UI.WpfUI.Utils.DTOs
{
    public class UndoStatesDTO
    {
        public object Container { get; set; }
        public object PropertyName { get; set; }
        public object InitialValue { get; set; }
        public object NewValue { get; set; }
    }
}
