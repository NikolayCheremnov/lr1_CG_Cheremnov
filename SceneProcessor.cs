using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lr1_CG_Cheremnov
{
    class SceneProcessor
    {
       public static WireObject PreparingProjection(WireObject body)
        {
            WireObject axes = new WireObject(@"Resources/axes.wo");
            WireObject projectedBody = new WireObject(@"Resources/cube.wo");

            return projectedBody;
        }
    }
}
