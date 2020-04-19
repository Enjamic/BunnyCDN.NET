using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enjamic.BunnyCdn.Models
{
    public class PullzoneEdgeRule
    {
        public string Guid { get; set; }
        public string ActionParameter1 { get; set; } = "";
        public string ActionParameter2 { get; set; } = "";
        public bool Enabled { get; set; } = true;
        public string Description { get; set; } = "";

        public int ActionType { get; set; }

        public int TriggerMatchingType { get; set; } = 0;
        public int Type { get; set; } = 0;

        public List<PullzoneEdgeRuleTrigger> Triggers { get; set; }
    }
}
