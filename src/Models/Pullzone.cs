using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enjamic.BunnyCdn.Models
{
    public class Pullzone
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OriginUrl { get; set; }
        public bool Enabled { get; set; }
        public List<PullzoneHostname> Hostnames { get; set; }
        public int StorageZoneId { get; set; }
        public List<string> AllowedReferrers { get; set; }
        public List<string> BlockedIps { get; set; }
        public bool EnableGeoZoneUS { get; set; }
        public bool EnableGeoZoneEU { get; set; }
        public bool EnableGeoZoneASIA { get; set; }
        public bool EnableGeoZoneSA { get; set; }
        public bool EnableGeoZoneAF { get; set; }
        public bool ZoneSecurityEnabled { get; set; }
        public string ZoneSecurityKey { get; set; }
        public bool ZoneSecurityIncludeHashRemoteIP { get; set; }
        public bool IgnoreQueryStrings { get; set; }
        public long MonthlyBandwidthLimit { get; set; }
        public long MonthlyBandwidthUsed { get; set; }
        public bool AddHostHeader { get; set; }
        public int Type { get; set; }
        public string CustomNginxConfig { get; set; }
        public List<string> AccessControlOriginHeaderExtensions { get; set; }
        public bool EnableAccessControlOriginHeader { get; set; }
        public bool DisableCookies { get; set; }
        public List<string> BudgetRedirectedCountries { get; set; }
        public List<string> BlockedCountries { get; set; }
        public bool EnableOriginShield { get; set; }
        public string OriginShieldZoneCode { get; set; }
        public bool DisableDualCache { get; set; }
        public int CacheControlMaxAgeOverride { get; set; }
        public int BurstSize { get; set; }
        public int RequestLimit { get; set; }
        public bool BlockRootPathAccess { get; set; }
        public int CacheQuality { get; set; }
        public double LimitRatePerSecond { get; set; }
        public double LimitRateAfter { get; set; }
        public int ConnectionLimitPerIPCount { get; set; }
        public double PriceOverride { get; set; }
        public bool AddCanonicalHeader { get; set; }
        public bool EnableLogging { get; set; }
        public bool IgnoreVaryHeader { get; set; }
        public bool EnableCacheSlice { get; set; }
        public List<PullzoneEdgeRule> EdgeRules { get; set; }
        public bool EnableWebPVary { get; set; }
        public bool AWSSigningEnabled { get; set; }

        public string AWSSigningKey { get; set; }
        public string AWSSigningRegionName { get; set; }
        public string AWSSigningSecret { get; set; }


    }
}
