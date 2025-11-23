namespace Service.Configurations
{
    public class PayOSSettings
    {
        public string ClientId { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ChecksumKey { get; set; } = string.Empty;
        public string Endpoint { get; set; } = "https://api-merchant.payos.vn/v2/payment-requests";

        public string IpnUrl { get; set; } = string.Empty;
        public string RedirectUrl { get; set; } = string.Empty;
    }
}

