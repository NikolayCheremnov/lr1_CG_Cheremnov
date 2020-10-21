using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace lr1_CG_Cheremnov
{
    class Edge
    {
        private Point3D p1;
        private Point3D p2;

        public Edge(Point3D p1, Point3D p2, string color)
        {
            this.P1 = p1;
            this.P2 = p2;
            this.Color = color;
        }

        public Point3D P2 { get => p2; set => p2 = value; }
        public Point3D P1 { get => p1; set => p1 = value; }
        public string Color { get; set; }


        // equality test
        public static bool operator ==(Edge edge1, Edge edge2)
        {
            return edge1.p1.X == edge2.p1.X && edge1.p1.Y == edge2.p1.Y && edge1.p1.Z == edge1.p1.Z &&
                edge1.p2.X == edge2.p2.X && edge1.p2.Y == edge2.p2.Y && edge1.p2.Z == edge2.p2.Z;
        }

        public static bool operator !=(Edge edge1, Edge edge2)
        {
            return !(edge1 == edge2);
        }


        public bool CheckContiguity(Edge edge)
        {
            // possible combinations: p1p1 p1p2 p2p1 p2p2
            if ((p1.X == edge.p1.X && p1.Y == edge.p1.Y && p1.Z == edge.p1.Z) ||
                (p1.X == edge.p2.X && p1.Y == edge.p2.Y && p1.Z == edge.p2.Z) ||
                (p2.X == edge.p1.X && p2.Y == edge.p1.Y && p2.Z == edge.p1.Z) ||
                (p2.X == edge.p2.X && p2.Y == edge.p2.Y && p2.Z == edge.p2.Z))
                return true;
            return false;
        }
        public void changeCoords(double new_x1, double new_y1, double new_z1, double new_x2, double new_y2, double new_z2)
        {
            p1.X = new_x1;
            p1.Y = new_y1;
            p1.Z = new_z1;
            p2.X = new_x2;
            p2.Y = new_y2;
            p2.Z = new_z2;
        }
    }
}
