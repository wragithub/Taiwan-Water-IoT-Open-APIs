using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senslink.Client.Models
{
    public class FileMetaData
    {
        public string name;
        public string description;
        public string[] keyworkds;
        public DateTime creationTime;
        public DateTime updateTime;
        public int scale;
        public string CRS;
        public float[] extent;
        public string authority;
        public string department;
        public string shareLevel;
    }
}
