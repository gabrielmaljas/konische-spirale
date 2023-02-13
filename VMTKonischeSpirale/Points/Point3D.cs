using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMTKonischeSpirale.Interfaces;

namespace VMTKonischeSpirale.Points
{
    public class Point3D : Point2D, ICsvFormattable
    {
        public double Z { get; set; }

        public Point3D(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;

        }

        
    }
}
