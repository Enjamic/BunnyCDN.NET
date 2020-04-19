using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enjamic.BunnyCdn.Models
{
    public class Statistics
    {
        public long TotalBandwidthUsed { get; set; }
        public long TotalRequestsServed { get; set; }
        public decimal CacheHitRate { get; set; }
        public Dictionary<DateTime, long> BandwidthUsedChart { get; set; }
        public Dictionary<DateTime, long> BandwidthCachedChart { get; set; }
        public Dictionary<DateTime, long> CacheHitRateChart { get; set; }
        public Dictionary<DateTime, long> RequestsServedChart { get; set; }
        public Dictionary<DateTime, long> PullRequestsPulledChart { get; set; }
        public Dictionary<DateTime, decimal> UserBalanceHistoryChart { get; set; }
        public Dictionary<DateTime, decimal> UserStorageUsedChart { get; set; }
        public Dictionary<string, long> GeoTrafficDistribution { get; set; }
        public Dictionary<DateTime, long> Error3xxChart { get; set; }
        public Dictionary<DateTime, long> Error4xxChart { get; set; }
        public Dictionary<DateTime, long> Error5xxChart { get; set; }
    }
}
