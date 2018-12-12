using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senslink.Client.Models
{
    public class NetCDFLayer
    {
        //Layers inside the NetCDF files
        public string Name { get; set; }
        public string Title { get; set; }
        public string SRS { get; set; }
        public BoundingBox BBox { get; set; }
        public List<string> Styles = new List<string>();
        public string ImageUrl { get; set; }
    }
}
