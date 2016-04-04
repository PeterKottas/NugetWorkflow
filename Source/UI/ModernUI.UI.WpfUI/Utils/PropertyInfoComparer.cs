using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.UI.WpfUI.Utils
{
    class PropertyInfoComparer : IEqualityComparer<PropertyInfo>
    {
        public bool Equals(PropertyInfo b1, PropertyInfo b2)
        {
            if (b2 == null && b1 == null)
                return true;
            else if (b1 == null | b2 == null)
                return false;
            else if (b1.Name == b2.Name)
                return true;
            else
                return false;
        }

        public int GetHashCode(PropertyInfo bx)
        {
            return bx.Name.GetHashCode();
        }
    }
}
