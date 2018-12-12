using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senslink.Client.Models
{
    public class JobInfo
    {
        public int id;
        public string argument;
        public string foldername;
        public string jobname;
        public string mainfile;
        public string priority;
        public string role;
        public string status;
        public DateTime revisedtime;
        public DateTime submittime;
        public DateTime uploadtime;
    }
}
