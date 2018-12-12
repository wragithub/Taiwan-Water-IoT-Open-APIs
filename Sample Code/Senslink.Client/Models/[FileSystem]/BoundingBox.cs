using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senslink.Client.Models
{
    public struct BoundingBox
    {
        //for NetCDF Layer
        public string minX { get; set; }
        public string maxX { get; set; }
        public string minY { get; set; }
        public string maxY { get; set; }
    }
}
