using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enjamic.BunnyCdn.Models
{
    public class BillingInfo
    {
        public decimal Balance { get; set; }
        public decimal ThisMonthCharges { get; set; }
        public decimal MonthlyChargesStorage { get; set; }
        public decimal MonthlyChargesEUTraffic { get; set; }
        public decimal MonthlyChargesUSTraffic { get; set; }
        public decimal MonthlyChargesASIATraffic { get; set; }
        public decimal MonthlyChargesSATraffic { get; set; }
    }
}
