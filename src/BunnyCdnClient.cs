using Enjamic.BunnyCdn.Actions;
using Enjamic.BunnyCdn.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Enjamic.BunnyCdn
{
    public interface IBunnyCdnClient
    {
        string SignUrl(string url, string securityKey);
        Pullzone GetPullzone(string pullzoneName);
        Pullzone GetPullzone(int pullzoneId);
        List<Pullzone> GetPullzones(bool forceRefresh = false);
        Statistics GetStatistics(int? pullZone, DateTime dateFrom, DateTime dateTo);
        BillingInfo GetBilling();
        Pullzone CreatePullzone(PullzoneCreateRequest createRequest);
        Pullzone UpdatePullzone(Pullzone pullzone);
        bool AddOrUpdateEdgeRule(int pullzoneId, PullzoneEdgeRule edgeRule);
        bool AddPullzoneHostname(PullzoneHostnameAddRequest addRequest);
        bool DeletePullzoneHostname(PullzoneHostnameAddRequest deleteRequest);
        bool AddPullzoneCertificate(string hostname);
        bool Purge(string url);
    }

    public class BunnyCdnClient : IBunnyCdnClient
    {
        private List<Pullzone> _pullZones;
        private string _apiKey;

        public BunnyCdnClient(string apiKey)
        {
            _apiKey = apiKey;
        }


        private  long ToUnixTime(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return 0;
            }
            else
            {
                return (long)((DateTime)dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
            }
        }


        public string SignUrl(string url, string securityKey)
        {
            // Set the time of expiry to one hour from now
            var expires = ToUnixTime(DateTime.UtcNow.AddMinutes(60));

            var uri = new Uri(url);

            // Generate the token
            MD5 md5 = MD5.Create();

            string hashableBase = securityKey + uri.PathAndQuery + expires;

            byte[] outpufBuffer = md5.ComputeHash(Encoding.UTF8.GetBytes(hashableBase));
            var token = Convert.ToBase64String(outpufBuffer);
            token = token.Replace("\n", "").Replace("+", "-").Replace("/", "_").Replace("=", "");
            return $"{url}?token={token}&expires={expires}";
        }

        public List<Pullzone> GetPullzones(bool forceRefresh = false)
        {
            if (_pullZones != null && !forceRefresh)
                return _pullZones;

            WebRequest request = WebRequest.Create("https://bunnycdn.com/api/pullzone");
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Timeout = 60 * 1000;
            request.Headers.Add("AccessKey", _apiKey);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var json = reader.ReadToEnd();
                        _pullZones = JsonConvert.DeserializeObject<List<Pullzone>>(json);
                        return _pullZones;
                    }
                }
            }
        }

        public Pullzone GetPullzone(string pullzoneName)
        {
            var pullzoneId = GetPullzones()?.FirstOrDefault(x => x.Name.ToLower() == pullzoneName)?.Id;

            if (pullzoneId == null || pullzoneId == 0)
            {
                return null;
            }
            return GetPullzone((int)pullzoneId);
        }


        public Pullzone GetPullzone(int pullzoneId)
        {
            WebRequest request = WebRequest.Create($"https://bunnycdn.com/api/pullzone/{pullzoneId}");
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Timeout = 60 * 1000;
            request.Headers.Add("AccessKey", _apiKey);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var json = reader.ReadToEnd();
                        return JsonConvert.DeserializeObject<Pullzone>(json);
                    }
                }
            }
        }


        public Pullzone CreatePullzone(PullzoneCreateRequest createRequest)
        {
            try
            {
                // Check if it already exists
                var pullzone = GetPullzone(createRequest.Name);
                if (pullzone != null)
                {
                    return pullzone;
                }

                // Clear cache of pullzone list so later this can be refreshed
                _pullZones = null;

                WebRequest request = WebRequest.Create("https://bunnycdn.com/api/pullzone");
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = 60 * 1000;
                request.Headers.Add("AccessKey", _apiKey);
                var reqJson = JsonConvert.SerializeObject(createRequest);

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(reqJson);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            var json = reader.ReadToEnd();
                            return JsonConvert.DeserializeObject<Pullzone>(json);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public Pullzone UpdatePullzone(Pullzone pullzone)
        {
            // Check if it already exists
            //var pullzone = GetPullzone(pullzoneId);
            //if (pullzone == null)
            //{
            //    return pullzone;
            //}


            WebRequest request = WebRequest.Create($"https://bunnycdn.com/api/pullzone/{pullzone.Id}");
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Timeout = 60 * 1000;
            request.Headers.Add("AccessKey", _apiKey);
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(JsonConvert.SerializeObject(pullzone));
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var json = reader.ReadToEnd();
                        return GetPullzone(pullzone.Id);
                    }
                }
            }
        }

        public bool AddOrUpdateEdgeRule(int pullzoneId, PullzoneEdgeRule edgeRule)
        {
            try
            {
                WebRequest request = WebRequest.Create($"https://bunnycdn.com/api/pullzone/{pullzoneId}/edgerules/addOrUpdate");
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = 60 * 1000;
                request.Headers.Add("AccessKey", _apiKey);
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(JsonConvert.SerializeObject(edgeRule));
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            var json = reader.ReadToEnd();
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool AddPullzoneHostname(PullzoneHostnameAddRequest addRequest)
        {
            WebRequest request = WebRequest.Create($"https://bunnycdn.com/api/pullzone/addHostname");
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Timeout = 60 * 1000;
            request.Headers.Add("AccessKey", _apiKey);
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(JsonConvert.SerializeObject(addRequest));
            }
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            var json = reader.ReadToEnd();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeletePullzoneHostname(PullzoneHostnameAddRequest deleteRequest)
        {
            WebRequest request = WebRequest.Create($"https://bunnycdn.com/api/pullzone/deleteHostname?id={deleteRequest.PullZoneId}&hostname={deleteRequest.Hostname}");
            request.Method = "DELETE";
            request.ContentType = "application/json";
            request.Timeout = 60 * 1000;
            request.Headers.Add("AccessKey", _apiKey);
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            var json = reader.ReadToEnd();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool AddPullzoneCertificate(string hostname)
        {
            WebRequest request = WebRequest.Create($"https://bunnycdn.com/api/pullzone/loadFreeCertificate?hostname={hostname}");
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Timeout = 60 * 1000;
            request.Headers.Add("AccessKey", _apiKey);
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            var json = reader.ReadToEnd();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public Statistics GetStatistics(int? pullZone, DateTime dateFrom, DateTime dateTo)
        {
            string apiParams = $"?dateFrom={dateFrom:yyyy-MM-dd}&dateTo={dateTo:yyyy-MM-dd}";
            if (pullZone != null)
            {
                apiParams += $"&pullZone={pullZone}";
            }

            Statistics statistics = new Statistics();

            WebRequest request = WebRequest.Create($"https://bunnycdn.com/api/statistics{apiParams}");
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Timeout = 60 * 1000;
            request.Headers.Add("AccessKey", _apiKey);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var json = reader.ReadToEnd();
                        statistics = JsonConvert.DeserializeObject<Statistics>(json);
                    }
                }
            }

            return statistics;

        }

        public BillingInfo GetBilling()
        {
            BillingInfo billingInfo = null;

            WebRequest request = WebRequest.Create($"https://bunnycdn.com/api/billing");
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Timeout = 60 * 1000;
            request.Headers.Add("AccessKey", _apiKey);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var json = reader.ReadToEnd();
                        billingInfo = JsonConvert.DeserializeObject<BillingInfo>(json);
                    }
                }
            }

            return billingInfo;

        }

        public bool Purge(string url)
        {
            var apiUrl = $"https://bunnycdn.com/api/purge?url=" + WebUtility.UrlEncode(url);
            WebRequest request = WebRequest.Create(apiUrl);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Timeout = 60 * 1000;
            request.Headers.Add("AccessKey", _apiKey);

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            var json = reader.ReadToEnd();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }

}
