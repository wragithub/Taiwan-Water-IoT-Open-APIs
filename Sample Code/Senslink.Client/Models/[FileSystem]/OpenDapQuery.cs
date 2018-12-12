using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senslink.Client.Models
{
    public struct OpenDapQuery
    {
        /// <summary>
        /// Parameters to query OpenDap netCdf files.
        /// </summary>
        public string FileName { get; set; }
        public string VariableName { get; set; }
        public long TimeStampInMin { get; set; }
        public float LeftUpCornerX { get; set; }
        public float LeftUpCornerY { get; set; }
        public float RightDownCornerX { get; set; }
        public float RightDownCornerY { get; set; }
    }
}
