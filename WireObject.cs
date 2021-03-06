﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using MathNet.Numerics.LinearAlgebra;

namespace lr1_CG_Cheremnov
{
    class WireObject
    {
        public List<Edge> Ridge { get; }
        public string Name { get; }
        public string Path { get; }

        // scales
        private int oxScale;
        private int oyScale;
        private int ozScale;

        // angles
        private double oxAngle;
        private double oyAngle;
        private double ozAngle;

        // shifts
        private double oxShift;
        private double oyShift;
        private double ozShift;

        // matrixes
        private Matrix<double> basicMatrix;

        public WireObject(string name, string woSrs)
        {
            if (name != null)
                this.Name = name;
            else
                this.Name = "undefined";

            // read object data
            Path = woSrs;

            Ridge = new List<Edge>();

            using (StreamReader srs = new StreamReader(woSrs))
            {
                string line;
                while ((line = srs.ReadLine()) != null)
                {
                    if (line[0] == '#')
                        continue;
                    string[] edgeDataStr = line.Split();
                    double x1 = double.Parse(edgeDataStr[0], System.Globalization.CultureInfo.InvariantCulture);
                    double y1 = double.Parse(edgeDataStr[1], System.Globalization.CultureInfo.InvariantCulture);
                    double z1 = double.Parse(edgeDataStr[2], System.Globalization.CultureInfo.InvariantCulture);
                    double x2 = double.Parse(edgeDataStr[3], System.Globalization.CultureInfo.InvariantCulture);
                    double y2 = double.Parse(edgeDataStr[4], System.Globalization.CultureInfo.InvariantCulture);
                    double z2 = double.Parse(edgeDataStr[5], System.Globalization.CultureInfo.InvariantCulture);

                    Ridge.Add(new Edge(new Point3D(x1, y1, z1), new Point3D(x2, y2, z2), edgeDataStr[6]));
                }
            }

            // parameter initialization
            oxScale = oyScale = ozScale = 100;
            oxAngle = oyAngle = ozAngle = 0;
            oxShift = oyShift = ozShift = 0;
            basicMatrix = Matrix<double>.Build.DenseOfArray(new double[,]
            {
                {1, 0, 0, 0 },
                {0, 1, 0, 0 },
                {0, 0, 1, 0 },
                {0, 0, 0, 1 }
            });

        }

        public void XY_Mirror()
        {
            basicMatrix[2, 2] = -1;
            Transformation(basicMatrix);
            RecoverBasicMatrix();
        }
        public void YZ_Mirror()
        {
            basicMatrix[0, 0] = -1;
            Transformation(basicMatrix);
            RecoverBasicMatrix();
        }

        public void ZX_Mirror()
        {
            basicMatrix[1, 1] = -1;
            Transformation(basicMatrix);
            RecoverBasicMatrix();
        }

        public void Shift(string oxShiftStr, string oyShiftStr, string ozShiftStr)
        {
            double oldOxShift = oxShift, oldOyShift = oyShift, oldOzShift = ozShift; // for recovering

            try
            {
                // data preparation
                double newOxShift = Convert.ToDouble(oxShiftStr);
                double newOyShift = Convert.ToDouble(oyShiftStr);
                double newOzShift = Convert.ToDouble(ozShiftStr);

                // set values
                basicMatrix[3, 0] = -oxShift + newOxShift;
                oxShift = newOxShift;
                basicMatrix[3, 1] = -oyShift + newOyShift;
                oyShift = newOyShift;
                basicMatrix[3, 2] = -ozShift + newOzShift;
                ozShift = newOzShift;

                // performing scaling
                Transformation(basicMatrix);
            }
            catch
            {
                // recover data
                oxShift = oldOxShift;
                oyShift = oldOyShift;
                ozShift = oldOzShift;
            }
            finally
            {
                // reset matrix
                RecoverBasicMatrix();
            }
        }

        public void Rotate(double oxAngleValue, double oyAngleValue, double ozAngleValue)
        {
            // recover canonical form
            basicMatrix[0, 0] = Math.Cos(-ozAngle);
            basicMatrix[0, 1] = Math.Sin(-ozAngle);
            basicMatrix[1, 0] = -Math.Sin(-ozAngle);
            basicMatrix[1, 1] = Math.Cos(-ozAngle);
            Transformation(basicMatrix);
            RecoverBasicMatrix();

            basicMatrix[0, 0] = Math.Cos(-oyAngle);
            basicMatrix[0, 2] = -Math.Sin(-oyAngle);
            basicMatrix[2, 0] = Math.Sin(-oyAngle);
            basicMatrix[2, 2] = Math.Cos(-oyAngle);
            Transformation(basicMatrix);
            RecoverBasicMatrix();

            basicMatrix[1, 1] = Math.Cos(-oxAngle);
            basicMatrix[1, 2] = Math.Sin(-oxAngle);
            basicMatrix[2, 1] = -Math.Sin(-oxAngle);
            basicMatrix[2, 2] = Math.Cos(-oxAngle);
            Transformation(basicMatrix);
            RecoverBasicMatrix();

            // rotation
            oxAngle = oxAngleValue;
            oyAngle = oyAngleValue;
            ozAngle = ozAngleValue;

            basicMatrix[1, 1] = Math.Cos(oxAngle);
            basicMatrix[1, 2] = Math.Sin(oxAngle);
            basicMatrix[2, 1] = -Math.Sin(oxAngle);
            basicMatrix[2, 2] = Math.Cos(oxAngle);
            Transformation(basicMatrix);
            RecoverBasicMatrix();

            basicMatrix[0, 0] = Math.Cos(oyAngle);
            basicMatrix[0, 2] = -Math.Sin(oyAngle);
            basicMatrix[2, 0] = Math.Sin(oyAngle);
            basicMatrix[2, 2] = Math.Cos(oyAngle);
            Transformation(basicMatrix);
            RecoverBasicMatrix();

            basicMatrix[0, 0] = Math.Cos(ozAngle);
            basicMatrix[0, 1] = Math.Sin(ozAngle);
            basicMatrix[1, 0] = -Math.Sin(ozAngle);
            basicMatrix[1, 1] = Math.Cos(ozAngle);
            Transformation(basicMatrix);
            RecoverBasicMatrix();
        }

        public void Scalling(string persentageOxValue, string persentageOyValue, string persentageOzValue)
        {
            int oldOxScale = oxScale, oldOyScale = oyScale, oldOzScale = ozScale; // for recovering

            try
            {
                // data preparation
                int newOxScalePercent = Convert.ToInt32(persentageOxValue);
                double newOxScaleMultiplier = Convert.ToDouble(persentageOxValue) / 100;
                int newOyScalePercent = Convert.ToInt32(persentageOyValue);
                double newOyScaleMultiplier = Convert.ToDouble(persentageOyValue) / 100;
                int newOzScalePercent = Convert.ToInt32(persentageOzValue);
                double newOzScaleMultiplier = Convert.ToDouble(persentageOzValue) / 100;

                // set values
                basicMatrix[0, 0] = newOxScaleMultiplier * 100 / oxScale;
                oxScale = newOxScalePercent;
                basicMatrix[1, 1] = newOyScaleMultiplier * 100 / oyScale;
                oyScale = newOyScalePercent;
                basicMatrix[2, 2] = newOzScaleMultiplier * 100 / ozScale;
                ozScale = newOzScalePercent;

                // performing scaling
                Transformation(basicMatrix);
            }
            catch {
                // recover data
                oxScale = oldOxScale;
                oyScale = oldOyScale;
                ozScale = oldOzScale;
            }
            finally
            {
                // reset matrix
                RecoverBasicMatrix();
            }
        }

        protected virtual void Transformation(Matrix<double> M)
        {
            for (int i = 0; i < Ridge.Count; i++)
            {
                Vector<double> p1Coords = Vector<double>.Build.DenseOfArray(new double[] { Ridge[i].P1.X, Ridge[i].P1.Y, Ridge[i].P1.Z, 1 });
                p1Coords = p1Coords * M;
                Vector<double> p2Coords = Vector<double>.Build.DenseOfArray(new double[] { Ridge[i].P2.X, Ridge[i].P2.Y, Ridge[i].P2.Z, 1 });
                p2Coords = p2Coords * M;
                Ridge[i].changeCoords(p1Coords[0], p1Coords[1], p1Coords[2], p2Coords[0], p2Coords[1], p2Coords[2]);
            }
        }

        private void RecoverBasicMatrix()
        {
            basicMatrix.Clear();
            basicMatrix[0, 0] = basicMatrix[1, 1] = basicMatrix[2, 2] = basicMatrix[3, 3] = 1;
        }

        // the determination of visibility edges
        public virtual bool EdgeVisibility(Edge edge, Vector3 projection_vec)
        {
            return true;
        }

        // cloning an object
        public virtual WireObject Clone()
        {
            return new WireObject(Name, Path);
        }
    }
}
