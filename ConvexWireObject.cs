using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace lr1_CG_Cheremnov
{
    class ConvexWireObject : WireObject
    {

        private Point3D internalPoint;
        private List<Face> Faces;

        public ConvexWireObject(string name, string woSrs)
            : base(name, woSrs)
        {
            internalPoint = new Point3D(1, 1, -1);

            // find faces
            Faces = new List<Face>();

            foreach(Edge currentEdge in Ridge)
            {
                Vector3 currentEdge_vec = new Vector3((float)(currentEdge.P2.X - currentEdge.P1.X), (float)(currentEdge.P2.Y - currentEdge.P1.Y), (float)(currentEdge.P2.Z - currentEdge.P1.Z));
                foreach(Edge adjacentEdge in Ridge)
                {
                    if ((currentEdge != adjacentEdge) && adjacentEdge.CheckContiguity(currentEdge))
                    {
                        Face potentialFace = new Face();
                        Vector3 adjacentEdge_vec = new Vector3((float)(adjacentEdge.P2.X - adjacentEdge.P1.X), (float)(adjacentEdge.P2.Y - adjacentEdge.P1.Y), (float)(adjacentEdge.P2.Z - adjacentEdge.P1.Z));
                        Vector3 mul = Vector3.Cross(adjacentEdge_vec, currentEdge_vec); // edge and current edge vec multiply
                        foreach(Edge added in Ridge)
                        {
                            if ((added != currentEdge) && (added != adjacentEdge))
                            {
                                Vector3 added_vec = new Vector3((float)(added.P2.X - added.P1.X), (float)(added.P2.Y - added.P1.Y), (float)(added.P2.Z - added.P1.Z));
                                if (Vector3.Dot(added_vec, mul) == 0)
                                    potentialFace.AddEdge(added);
                            }
                        }
                        if (potentialFace.Ridge.Count != 0) // then we found the face
                        {
                            potentialFace.AddEdge(adjacentEdge);
                            potentialFace.AddEdge(currentEdge);
                            potentialFace.RemoveFakeEdges();
                            AddFace(potentialFace);
                        }
                    }
                }
            }
        }

        protected override void Transformation(Matrix<double> M)
        {
            for (int i = 0; i < Ridge.Count; i++)
            {
                Vector<double> p1Coords = Vector<double>.Build.DenseOfArray(new double[] { Ridge[i].P1.X, Ridge[i].P1.Y, Ridge[i].P1.Z, 1 });
                p1Coords = p1Coords * M;
                Vector<double> p2Coords = Vector<double>.Build.DenseOfArray(new double[] { Ridge[i].P2.X, Ridge[i].P2.Y, Ridge[i].P2.Z, 1 });
                p2Coords = p2Coords * M;
                Ridge[i].changeCoords(p1Coords[0], p1Coords[1], p1Coords[2], p2Coords[0], p2Coords[1], p2Coords[2]);
            }
            Vector<double> oCoords = Vector<double>.Build.DenseOfArray(new double[] { internalPoint.X, internalPoint.Y, internalPoint.Z, 1 });
            oCoords = oCoords * M;
            internalPoint.X = oCoords[0];
            internalPoint.Y = oCoords[1];
            internalPoint.Z = oCoords[2];
        }

        // the determination of visibility edges overload
        public override bool EdgeVisibility(Edge edge, Vector3 projection_vec)
        {
            // prepare faces
            for (int i = 0; i < Faces.Count; i++)
            {
                Faces[i].InitializeNormal(internalPoint);
                Faces[i].InitializeVisibilityTag(projection_vec);
            }

            // check edge visibility
            foreach (Face face in Faces)
                if (face.VisibilityTag && face.FaceContainsEdge(edge))
                    return true;
            return false;
        }

        public void AddFace(Face added)
        {
            foreach (Face face in Faces)
                if (added == face)
                    return;
            Faces.Add(added);
        }

        // cloning an object
        public override WireObject Clone()
        {
            return new ConvexWireObject(Name, Path);
        }
    }
}
