using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Common.Momo.Model
{
    public class MomoPaymentResponse
    {
        public int ResultCode { get; set; }
        public string Message { get; set; }
        public string PayUrl { get; set; }
        public string DeepLink { get; set; }
        public string QrCodeUrl { get; set; }
        public string RequestId { get; set; }
        public string OrderId { get; set; }
        public long Amount { get; set; }
        public string Signature { get; set; }
    }
}

