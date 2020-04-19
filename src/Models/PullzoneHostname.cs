using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enjamic.BunnyCdn.Models
{
    public class PullzoneHostname
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public bool ForceSSL { get; set; }
        public bool IsSystemHostname { get; set; }
        public bool HasCertificate { get; set; }
    }
}
