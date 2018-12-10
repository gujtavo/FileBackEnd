using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEndSimulation
{
    public class FileRequest
    {
        public int accion { get; set; }
        public FileModel file { get; set; }
    }

    public class FileModel
    {
        public string id { get; set; }
        public string filename { get; set; }
        public string file { get; set; }
    }
}
