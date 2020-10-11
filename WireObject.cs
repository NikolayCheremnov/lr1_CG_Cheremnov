using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace lr1_CG_Cheremnov
{
    class WireObject
    {
        private List<((double x1, double y1, double z1) p1, (double x2, double y2, double z2) p2)> ridge;  // body frame

        public WireObject(string woSrs)
        {
            ridge = new List<((double x1, double y1, double z1) p1, (double x2, double y2, double z2) p2)>();

            using (StreamReader srs = new StreamReader(woSrs))
            {
                string line;
                while ((line = srs.ReadLine()) != null)
                {
                    string[] coords = line.Split();
                    ridge.Add(((Convert.ToDouble(coords[0]), Convert.ToDouble(coords[1]), Convert.ToDouble(coords[2])),
                        (Convert.ToDouble(coords[3]), Convert.ToDouble(coords[4]), Convert.ToDouble(coords[5]))));
                }
            }
        }
    }
}
