using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewFlocking.Things.StateIndicators
{
    class Fear : StateIndicator
    {
        public Fear(World aWorld, Thing parent)
            : base(aWorld, parent)
        {
        }

        public Fear(World aWorld, Thing parent, int ttl)
            : base(aWorld, parent, ttl)
        {
        }
    }
}
