using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using MathNet.Numerics.LinearAlgebra;

namespace lr1_CG_Cheremnov
{
    class SceneProcessor
    {
        
        // projection
        public static WireObject PreparingFreeProjection(WireObject wo)
        {
            WireObject projected_wo = new WireObject(wo.Name + "_free_projection", wo.Path);
  
            for (int i = 0; i < projected_wo.Ridge.Count; i++)
            {
                Vector<double> p1Coords = Vector<double>.Build.DenseOfArray(new double[] { wo.Ridge[i].p1.x, wo.Ridge[i].p1.y, wo.Ridge[i].p1.z, 1 });
                p1Coords = p1Coords * free_projection_matr;
                Vector<double> p2Coords = Vector<double>.Build.DenseOfArray(new double[] { wo.Ridge[i].p2.x, wo.Ridge[i].p2.y, wo.Ridge[i].p2.z, 1 });
                p2Coords = p2Coords * free_projection_matr;
                projected_wo.ChangeCoords(i, (p1Coords[0], p1Coords[1], p1Coords[2]), (p2Coords[0], p2Coords[1], p2Coords[2]));
            }

            return projected_wo;
        }

        // projection matrices
        private static Matrix<double> free_projection_matr = Matrix<double>.Build.DenseOfArray(new double[,] { 
        {1, 0, 0, 0},
        {0, 1, 0, 0 },
        {Math.Cos(Math.PI / 4), Math.Cos(Math.PI / 4), 0, 0 },
        {0, 0, 0, 1 }});

        // preparation of drawing
        public static Line PreparatingLine(string color, double x1, double y1, double x2, double y2, bool isClone)
        {
            Line line = new Line();
            line.Visibility = System.Windows.Visibility.Visible;
            line.StrokeThickness = 1;
            SolidColorBrush brush = (SolidColorBrush)new BrushConverter().ConvertFromString(color);
            if (isClone)
            {
                line.StrokeDashArray = new DoubleCollection();
                line.StrokeDashArray.Add(4);
                line.StrokeDashArray.Add(2);
            }
            line.Stroke = brush;
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
            return line;
        }

    }
}
