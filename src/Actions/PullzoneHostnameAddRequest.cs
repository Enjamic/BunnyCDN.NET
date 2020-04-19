using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enjamic.BunnyCdn.Actions
{
    public class PullzoneHostnameAddRequest
    {
        public int PullZoneId { get; set; }
        public string Hostname { get; set; }
    }
}
