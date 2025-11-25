using Service.Common.Momo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Common.Momo.MomoServices
{
    public interface IMoMoService
    {
        Task<(string PaymentUrl, string RequestId)> CreatePaymentUrlAsync(
            string orderId,
            string orderInfo,
            long amount,
            string extraData = "");
    }
}

