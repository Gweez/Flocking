using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Feesh.Things.StateIndicators
{
    class Relaxed : StateIndicator
    {
        public Relaxed(World aWorld, Thing parent)
            : base(aWorld, parent)
        {
            color = Color.Green;
        }
    }
}
