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
        public string PropertyName { get; set; }
        public Action Undo { get; set; }
        public Action Redo { get; set; }
    }
}
