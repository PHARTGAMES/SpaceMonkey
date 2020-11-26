using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericTelemetryProvider
{
    public class FilterBase
    {

        public virtual float Filter(float value)
        {
            return value;
        }

    }
}
