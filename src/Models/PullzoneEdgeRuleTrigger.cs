using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enjamic.BunnyCdn.Models
{
    public class PullzoneEdgeRuleTrigger
    {
        public string Guid { get; set; }
        public string Parameter1 { get; set; } = "";
        public int Type { get; set; } = 0;
        public int TriggerMatchingType { get; set; } = 0;
        public List<string> PatternMatches { get; set; }
    }
}
