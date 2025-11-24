namespace Service.DTOs
{
    public class PaymentCallbackResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string? OrderId { get; set; }
        public string? TransactionNo { get; set; }
        public string? VnPayResponseCode { get; set; }
    }
}

