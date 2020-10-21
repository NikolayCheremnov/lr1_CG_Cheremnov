using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace lr1_CG_Cheremnov
{
    class Face
    {
        public Face()
        {
            Ridge = new List<Edge>();
        }

        public bool VisibilityTag { get; set; }
        public List<Edge> Ridge { get; }
        public Vector3 ExternalNormal { get; set; }
        public void AddEdge(Edge AddedEdge)
        {
            foreach (Edge edge in Ridge)
                if (AddedEdge == edge)
                    return;
            Ridge.Add(AddedEdge);
        }


        // checking whether an edge belongs to a face
        public bool FaceContainsEdge(Edge checkingEdge)
        {
            foreach (Edge edge in Ridge)
                if (edge == checkingEdge)
                    return true;
            return false;
        }
        // initialize visibility tag
        public void InitializeVisibilityTag(Vector3 projection_vec)
        {
            if (Vector3.Dot(ExternalNormal, projection_vec) <= 0)
                VisibilityTag = true;
            else
                VisibilityTag = false;
        }

        // initialize external normal
        public void InitializeNormal(Point3D internalPoint)
        {
            Edge edge1 = Ridge[Ridge.Count-1];
            Edge edge2 = Ridge[Ridge.Count - 2];
            Vector3 edge1_vec = new Vector3((float)(edge1.P2.X - edge1.P1.X), (float)(edge1.P2.Y - edge1.P1.Y), (float)(edge1.P2.Z - edge1.P1.Z));
            Vector3 edge2_vec = new Vector3((float)(edge2.P2.X - edge2.P1.X), (float)(edge2.P2.Y - edge2.P1.Y), (float)(edge2.P2.Z - edge2.P1.Z));
            ExternalNormal = Vector3.Cross(edge1_vec, edge2_vec);
            ExternalNormal = Vector3.Normalize(ExternalNormal);
            Vector3 external_vec = new Vector3((float)(internalPoint.X - edge1.P1.X), (float)(internalPoint.Y - edge1.P1.Y), (float)(internalPoint.Z - edge1.P1.Z));
            if (Vector3.Dot(external_vec, ExternalNormal) > 0)
                ExternalNormal = Vector3.Negate(ExternalNormal);
        }

        // excess edges removing
        public void RemoveFakeEdges()
        {
            int i = 0;
            while (i < Ridge.Count) {
                bool isFake = true;
                foreach (Edge edge in Ridge)
                    if ((edge != Ridge[i]) && Ridge[i].CheckContiguity(edge))
                    {
                        isFake = false;
                        break;
                    }
                if (isFake)
                    Ridge.RemoveAt(i);
                else
                    i++;
            }
        }

        // equality test
        public static bool operator ==(Face face1, Face face2)
        {
            foreach(Edge edge1 in face1.Ridge)
            {
                bool flag = false;
                foreach(Edge edge2 in face2.Ridge)
                    if (edge1 == edge2)
                    {
                        flag = true;
                        break;
                    }
                if (!flag)
                    return false;
            }
            return true;
        }

        public static bool operator !=(Face face1, Face face2)
        {
            return !(face1 == face2);
        }
    }
}
