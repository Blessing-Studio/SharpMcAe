using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMcAe.Class {
    public struct Color {
        public double R { get; set; }

        public double G { get; set; }

        public double B { get; set; }

        public Color(double r, double g, double b) {
            R = r;
            G = g;
            B = b;
        }
    }
}
