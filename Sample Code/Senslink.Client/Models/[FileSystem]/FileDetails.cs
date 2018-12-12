using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senslink.Client.Models
{

    public struct FileDetails
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string Owner { get; set; }
        public string DownloadLink { get; set; }
        public string DeleteLink { get; set; }
        public string SharedLevel { get; set; }
        public string WmsGetCapabilitiesLink { get; set; }
        public string PngImagesInfos { get; set; }
        public string NetcdfASCIIFormatLink { get; set; }
        public string Godiva3ViewerLink { get; set;}
    }
}
